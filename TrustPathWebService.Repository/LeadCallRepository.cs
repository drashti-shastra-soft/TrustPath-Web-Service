using TrustPathWebService.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using TrustPathWebService.Infrastructure.IRepository;

namespace TrustPathWebService.Repository
{
    public class LeadCallRepository : ILeadCallRepository
    {
        public Domain.LeadCallLister GetAll(Domain.LeadCallLister mLister)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var query = from lc in context.LeadCalls
                                join c in context.Companies on lc.CompanyId equals c.Id into joinUC
                                from c in joinUC.DefaultIfEmpty()
                                join at in context.AuditTrails on lc.AuditId equals at.Id
                                where at.DeletedBy == null && at.DeletedDate == null
                                select new
                                {
                                    Id = lc.Id,
                                    CompanyId = lc.CompanyId,
                                    LeadId = lc.LeadId,
                                    DateTime = lc.DateTime,
                                    CommunicationMode = lc.CommunicationMode,
                                    CallStatus = lc.CallStatus,
                                    Message = lc.Message,
                                    Description = lc.Description,
                                    UpComeFollowUp = lc.UpComeFollowUp,
                                    Company = c.Name
                                };
                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.CommunicationMode.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.CommunicationMode) && x.CommunicationMode.ToUpper().Contains(criteria.CommunicationMode.ToUpper()));

                        if (criteria.Description.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Description) && x.Description.ToUpper().Contains(criteria.Description.ToUpper()));

                        if (criteria.LeadId > 0)
                            query = query.Where(x => x.LeadId == criteria.LeadId);

                        if (criteria.CompanyId > 0)
                            query = query.Where(x => x.CompanyId == criteria.CompanyId);
                    }
                    // Switch to in-memory for complex string filters
                    var efLeadcall = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efLeadcall.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efLeadcall = efLeadcall.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.LeadCall>>(JsonSerializer.Serialize(efLeadcall.ToList()));
                }
            }
            catch
            {
                throw;
            }

            return mLister;

        }

        public Domain.LeadCall Upsert(Domain.LeadCall mLeadCall)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efLeadCall = context.LeadCalls.FirstOrDefault(x => x.Id == mLeadCall.Id);
                    if (efLeadCall == null)
                    {
                        var audit = new Persistance.AuditTrail
                        {
                            CreatedBy = mLeadCall.CreatedBy,
                            CreatedDate = DateTime.Now
                        };

                        context.AuditTrails.Add(audit);
                        context.SaveChanges();

                        efLeadCall = new Persistance.LeadCall
                        {
                            AuditId = audit.Id
                        };
                        context.LeadCalls.Add(efLeadCall);
                    }
                    else
                    {
                        var audit = context.AuditTrails.Find(efLeadCall.AuditId);
                        if (audit != null)
                        {
                            audit.LastModifiedBy = mLeadCall.LastModifiedBy;
                            audit.LastModifiedDate = DateTime.Now;
                        }
                    }

                    efLeadCall.LeadId = mLeadCall.LeadId;
                    efLeadCall.CompanyId = mLeadCall.CompanyId;
                    efLeadCall.DateTime = DateTime.Now;
                    efLeadCall.CommunicationMode = mLeadCall.CommunicationMode;
                    efLeadCall.CallStatus = mLeadCall.CallStatus;
                    efLeadCall.Message = mLeadCall.Message;
                    efLeadCall.Description = mLeadCall.Description;
                    efLeadCall.UpComeFollowUp = mLeadCall.UpComeFollowUp;

                    context.SaveChanges();
                    mLeadCall.Id = efLeadCall.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mLeadCall;
        }

        public Domain.LeadCall Get(int id)
        {
            var mLeadCall = new Domain.LeadCall();
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efLeadCall = (from lc in context.LeadCalls
                                      where lc.Id == id
                                      select new
                                      {
                                          Id = lc.Id,
                                          CompanyId = lc.CompanyId,
                                          LeadId = lc.LeadId,
                                          DateTime = lc.DateTime,
                                          CommunicationMode = lc.CommunicationMode,
                                          CallStatus = lc.CallStatus,
                                          Message = lc.Message,
                                          Description = lc.Description,
                                          UpComeFollowUp = lc.UpComeFollowUp,
                                      }).FirstOrDefault();

                    if (efLeadCall != null)
                    {
                        mLeadCall = JsonSerializer.Deserialize<Domain.LeadCall>(JsonSerializer.Serialize(efLeadCall));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mLeadCall;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efLeadCall = context.LeadCalls.FirstOrDefault(x => x.Id == id);
                    if (efLeadCall != null)
                    {
                        var audit = context.AuditTrails.Find(efLeadCall.AuditId);
                        if (audit != null)
                        {
                            audit.DeletedBy = loginUserId;
                            audit.DeletedDate = DateTime.Now;
                        }
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Domain.LeadCall WhatsAppMessage(Domain.LeadCall mLeadCall)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efLeadCall = context.LeadCalls.FirstOrDefault(x => x.Id == mLeadCall.Id);
                    if (efLeadCall == null)
                    {
                        var audit = new Persistance.AuditTrail
                        {
                            CreatedBy = mLeadCall.CreatedBy,
                            CreatedDate = DateTime.Now
                        };

                        context.AuditTrails.Add(audit);
                        context.SaveChanges();

                        efLeadCall = new Persistance.LeadCall
                        {
                            AuditId = audit.Id
                        };
                        context.LeadCalls.Add(efLeadCall);
                    }
                    efLeadCall.LeadId = mLeadCall.LeadId;
                    efLeadCall.Message = mLeadCall.Message;
                    efLeadCall.DateTime = DateTime.Now;
                    efLeadCall.CommunicationMode = mLeadCall.CommunicationMode;
                    context.SaveChanges();
                    mLeadCall.Id = efLeadCall.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mLeadCall;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.Json;
using TrustPathWebService.Infrastructure.IRepository;
using TrustPathWebService.Infrastructure.Utils;

namespace TrustPathWebService.Repository
{
    public class LeadRepository : ILeadRepository
    {
        public Domain.LeadLister GetAll(Domain.LeadLister mLister)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var query = from l in context.Leads
                                join c in context.Companies on l.CompanyId equals c.Id into joinUC
                                from c in joinUC.DefaultIfEmpty()
                                join bt in context.BusinessTypes on l.BusinessTypeId equals bt.Id
                                join s in context.Sources on l.SourceId equals s.Id
                                join at in context.AuditTrails on l.AuditId equals at.Id
                                let latestCall = context.LeadCalls.Where(x => x.LeadId == l.Id).OrderByDescending(x => x.UpComeFollowUp).FirstOrDefault()
                                where at.DeletedBy == null && at.DeletedDate == null
                                orderby bt.Id descending
                                select new
                                {
                                    Id = l.Id,
                                    CompanyId = l.CompanyId,
                                    Name = l.Name,
                                    PhoneNumber = l.PhoneNumber,
                                    EmailAddress = l.EmailAddress,
                                    StateId = l.StateId,
                                    BusinessTypeId = l.BusinessTypeId,
                                    Requirement = l.Requirement,
                                    AssignLead = l.AssignLead,
                                    SourceId = l.SourceId,
                                    LeadStatus = l.LeadStatus,
                                    Source = s.Name,
                                    BusinessType = bt.Name,
                                    Company = c.Name,
                                    UpComeFollowUp = latestCall != null ? latestCall.UpComeFollowUp : null
                                };
                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.Name.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToUpper().Contains(criteria.Name.ToUpper()));

                        if (criteria.PhoneNumber.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.ToUpper().Contains(criteria.PhoneNumber.ToUpper()));

                        if (criteria.EmailAddress.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.EmailAddress) && x.EmailAddress.ToUpper().Contains(criteria.EmailAddress.ToUpper()));

                        if (criteria.CompanyId > 0)
                            query = query.Where(x => x.CompanyId == criteria.CompanyId);

                        if (criteria.LeadStatus.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.LeadStatus) && x.LeadStatus.ToUpper().Contains(criteria.LeadStatus.ToUpper()));

                        if (criteria.UpComeFollowUp.HasValue)
                        {
                            var selectedDate = criteria.UpComeFollowUp.Value.Date;

                            query = query.Where(x => x.UpComeFollowUp.HasValue && x.UpComeFollowUp.Value.Year == selectedDate.Year && x.UpComeFollowUp.Value.Month == selectedDate.Month && x.UpComeFollowUp.Value.Day == selectedDate.Day );
                        }
                    }
                    // Switch to in-memory for complex string filters
                    var efLead = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efLead.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efLead = efLead.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.Lead>>(JsonSerializer.Serialize(efLead.ToList()));
                }
            }
            catch
            {
                throw;
            }

            return mLister;

        }

        public Domain.Lead Upsert(Domain.Lead mLead)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efLead = context.Leads.FirstOrDefault(x => x.Id == mLead.Id);
                    if (efLead == null)
                    {
                        var audit = new Persistance.AuditTrail
                        {
                            CreatedBy = mLead.CreatedBy,
                            CreatedDate = DateTime.Now
                        };
                        context.AuditTrails.Add(audit);
                        context.SaveChanges();

                        efLead = new Persistance.Lead
                        {
                            AuditId = audit.Id
                        };
                        context.Leads.Add(efLead);
                    }
                    else
                    {
                        var audit = context.AuditTrails.Find(efLead.AuditId);
                        if (audit != null)
                        {
                            audit.LastModifiedBy = mLead.LastModifiedBy;
                            audit.LastModifiedDate = DateTime.Now;
                        }
                    }

                    efLead.CompanyId = mLead.CompanyId;
                    efLead.Name = mLead.Name;
                    efLead.PhoneNumber = mLead.PhoneNumber;
                    efLead.EmailAddress = mLead.EmailAddress;
                    efLead.StateId = mLead.StateId;
                    efLead.BusinessTypeId = mLead.BusinessTypeId;
                    efLead.Requirement = mLead.Requirement;
                    efLead.SourceId = mLead.SourceId;
                    efLead.AssignLead = mLead.AssignLead;
                    efLead.LeadStatus = mLead.LeadStatus;

                    context.SaveChanges();
                    mLead.Id = efLead.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mLead;
        }

        public Domain.Lead Get(int id)
        {
            var mLead = new Domain.Lead();
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efLead = (from l in context.Leads
                                  where l.Id == id
                                  select new
                                  {
                                      Id = l.Id,
                                      CompanyId = l.CompanyId,
                                      Name = l.Name,
                                      PhoneNumber = l.PhoneNumber,
                                      EmailAddress = l.EmailAddress,
                                      StateId = l.StateId,
                                      BusinessTypeId = l.BusinessTypeId,
                                      Requirement = l.Requirement,
                                      AssignLead = l.AssignLead,
                                      SourceId = l.SourceId,
                                      LeadStatus = l.LeadStatus,
                                  }).FirstOrDefault();

                    if (efLead != null)
                    {
                        mLead = JsonSerializer.Deserialize<Domain.Lead>(JsonSerializer.Serialize(efLead));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mLead;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efLead = context.Leads.FirstOrDefault(x => x.Id == id);
                    if (efLead != null)
                    {
                        var audit = context.AuditTrails.Find(efLead.AuditId);
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
    }
}

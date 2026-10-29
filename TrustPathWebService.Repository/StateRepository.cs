using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TrustPathWebService.Infrastructure.IRepository;
using TrustPathWebService.Infrastructure.Utils;

namespace TrustPathWebService.Repository
{
    public class StateRepository : IStateRepository
    {
        public Domain.StateLister GetAll(Domain.StateLister mLister)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var query = from s in context.States
                                join c in context.Companies on s.CompanyId equals c.Id into joinUC
                                from c in joinUC.DefaultIfEmpty()
                                join at in context.AuditTrails on s.AuditId equals at.Id
                                where at.DeletedBy == null && at.DeletedDate == null
                                select new
                                {
                                    Id = s.Id,
                                    CompanyId = s.CompanyId,
                                    Name = s.Name,
                                    Code = s.Code,
                                    IsActive = s.IsActive,
                                    Company = c.Name
                                };
                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.Name.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToUpper().Contains(criteria.Name.ToUpper()));

                        if (criteria.Code.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Code) && x.Code.ToUpper().Contains(criteria.Code.ToUpper()));

                        if (criteria.CompanyId > 0)
                            query = query.Where(x => x.CompanyId == criteria.CompanyId);

                        if (criteria.Status.IsNotNullOrEmpty())
                        {
                            if (criteria.Status == "Active")
                                query = query.Where(x => x.IsActive);
                            else if (criteria.Status == "Disabled")
                                query = query.Where(x => !x.IsActive);
                        }
                    }
                    // Switch to in-memory for complex string filters
                    var efState = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efState.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efState = efState.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;

                    mLister.List = JsonSerializer.Deserialize<List<Domain.State>>(JsonSerializer.Serialize(efState.ToList()));
                }
            }
            catch
            {
                throw;
            }

            return mLister;
        }

        public Domain.State Upsert(Domain.State mState)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efState = context.States.FirstOrDefault(x => x.Id == mState.Id);
                    if (efState == null)
                    {
                        var audit = new Persistance.AuditTrail
                        {
                            CreatedBy = mState.CreatedBy,
                            CreatedDate = DateTime.Now
                        };
                        context.AuditTrails.Add(audit);
                        context.SaveChanges();

                        efState = new Persistance.State
                        {
                            AuditId = audit.Id
                        };
                        context.States.Add(efState);
                    }
                    else
                    {
                        var audit = context.AuditTrails.Find(efState.AuditId);
                        if (audit != null)
                        {
                            audit.LastModifiedBy = mState.LastModifiedBy;
                            audit.LastModifiedDate = DateTime.Now;
                        }
                    }

                    efState.CompanyId = mState.CompanyId;
                    efState.Name = mState.Name;
                    efState.Code = mState.Code;
                    efState.IsActive = mState.IsActive;

                    context.SaveChanges();
                    mState.Id = efState.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mState;
        }

        public Domain.State Get(int id)
        {
            var mState = new Domain.State();
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efState = (from s in context.States
                                   where s.Id == id
                                   select new
                                   {
                                       Id = s.Id,
                                       CompanyId = s.CompanyId,
                                       Code = s.Code,
                                       Name = s.Name,
                                       IsActive = s.IsActive,
                                   }).FirstOrDefault();

                    if (efState != null)
                    {
                        mState = JsonSerializer.Deserialize<Domain.State>(JsonSerializer.Serialize(efState));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mState;

        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efState = context.States.FirstOrDefault(x => x.Id == id);
                    if (efState != null)
                    {
                        var audit = context.AuditTrails.Find(efState.AuditId);
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

        public List<Domain.State> GetActiveState()
        {
            var mState = new List<Domain.State>();
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efState = from s in context.States
                                  join at in context.AuditTrails on s.AuditId equals at.Id
                                  where at.DeletedBy == null && at.DeletedDate == null
                                  select new
                                  {
                                      Id = s.Id,
                                      Name = s.Name,
                                      AuditId = s.AuditId
                                  };

                    mState = JsonSerializer.Deserialize<List<Domain.State>>(JsonSerializer.Serialize(efState.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mState;
        }
    }
}

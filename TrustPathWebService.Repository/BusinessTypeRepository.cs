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
    public class BusinessTypeRepository : IBusinessTypeRepository
    {
        public Domain.BusinessTypeLister GetAll(Domain.BusinessTypeLister mLister)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var query = from bt in context.BusinessTypes
                                join c in context.Companies on bt.CompanyId equals c.Id into joinUC
                                from c in joinUC.DefaultIfEmpty()
                                join at in context.AuditTrails on bt.AuditId equals at.Id
                                where at.DeletedBy == null && at.DeletedDate == null
                                select new
                                {
                                    Id = bt.Id,
                                    CompanyId = bt.CompanyId,
                                    Name = bt.Name,
                                    IsActive = bt.IsActive,
                                    Company = c.Name
                                };
                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.Name.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToUpper().Contains(criteria.Name.ToUpper()));

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
                    var efBusinessType = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efBusinessType.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efBusinessType = efBusinessType.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.BusinessType>>(JsonSerializer.Serialize(efBusinessType.ToList()));
                }
            }
            catch
            {
                throw;
            }

            return mLister;

        }

        public Domain.BusinessType Upsert(Domain.BusinessType mBusinessType)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efBusinessType = context.BusinessTypes.FirstOrDefault(x => x.Id == mBusinessType.Id);
                    if (efBusinessType == null)
                    {
                        var audit = new Persistance.AuditTrail
                        {
                            CreatedBy = mBusinessType.CreatedBy,
                            CreatedDate = DateTime.Now
                        };
                        context.AuditTrails.Add(audit);
                        context.SaveChanges();

                        efBusinessType = new Persistance.BusinessType
                        {
                            AuditId = audit.Id
                        };
                        context.BusinessTypes.Add(efBusinessType);
                    }
                    else
                    {
                        var audit = context.AuditTrails.Find(efBusinessType.AuditId);
                        if (audit != null)
                        {
                            audit.LastModifiedBy = mBusinessType.LastModifiedBy;
                            audit.LastModifiedDate = DateTime.Now;
                        }
                    }

                    efBusinessType.CompanyId = mBusinessType.CompanyId;
                    efBusinessType.Name = mBusinessType.Name;
                    efBusinessType.IsActive = mBusinessType.IsActive;

                    context.SaveChanges();
                    mBusinessType.Id = efBusinessType.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mBusinessType;
        }

        public Domain.BusinessType Get(int id)
        {
            var mBusinessType = new Domain.BusinessType();
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efBusinessType = (from bt in context.BusinessTypes
                                    where bt.Id == id
                                    select new
                                    {
                                        Id = bt.Id,
                                        CompanyId = bt.CompanyId,
                                        Name = bt.Name,
                                        IsActive = bt.IsActive,
                                    }).FirstOrDefault();

                    if (efBusinessType != null)
                    {
                        mBusinessType = JsonSerializer.Deserialize<Domain.BusinessType>(JsonSerializer.Serialize(efBusinessType));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mBusinessType;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efBusinessType = context.BusinessTypes.FirstOrDefault(x => x.Id == id);
                    if (efBusinessType != null)
                    {
                        var audit = context.AuditTrails.Find(efBusinessType.AuditId);
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

        public List<Domain.BusinessType> GetActiveBusinessType()
        {
            var mBusinessType = new List<Domain.BusinessType>();
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efBusiness = from bt in context.BusinessTypes
                                     join at in context.AuditTrails on bt.AuditId equals at.Id
                                     where at.DeletedBy == null && at.DeletedDate == null
                                     select new
                                      {
                                          Id = bt.Id,
                                          Name = bt.Name,
                                          AuditId = bt.AuditId
                                      };

                    mBusinessType = JsonSerializer.Deserialize<List<Domain.BusinessType>>(JsonSerializer.Serialize(efBusiness.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mBusinessType;
        }
    }
}

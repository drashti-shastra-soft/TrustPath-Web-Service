using TrustPathWebService.Infrastructure.IRepository;
using TrustPathWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace TrustPathWebService.Repository
{
    public class SourceRepository : ISourceRepository
    {
        public Domain.SourceLister GetAll(Domain.SourceLister mLister)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var query = from s in context.Sources
                                join c in context.Companies on s.CompanyId equals c.Id into joinUC
                                from c in joinUC.DefaultIfEmpty()
                                join at in context.AuditTrails on s.AuditId equals at.Id
                                where at.DeletedBy == null && at.DeletedDate == null
                                select new
                                {
                                    Id = s.Id,
                                    CompanyId = s.CompanyId,
                                    Name = s.Name,
                                    IsActive = s.IsActive,
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
                    var efSource = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efSource.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efSource = efSource.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.Source>>(JsonSerializer.Serialize(efSource.ToList()));
                }
            }
            catch
            {
                throw;
            }

            return mLister;

        }

        public Domain.Source Upsert(Domain.Source mSource)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efSource = context.Sources.FirstOrDefault(x => x.Id == mSource.Id);
                    if (efSource == null)
                    {
                        var audit = new Persistance.AuditTrail
                        {
                            CreatedBy = mSource.CreatedBy,
                            CreatedDate = DateTime.Now
                        };
                        context.AuditTrails.Add(audit);
                        context.SaveChanges();

                        efSource = new Persistance.Source
                        {
                            AuditId = audit.Id
                        };
                        context.Sources.Add(efSource);
                    }
                    else
                    {
                        var audit = context.AuditTrails.Find(efSource.AuditId);
                        if (audit != null)
                        {
                            audit.LastModifiedBy = mSource.LastModifiedBy;
                            audit.LastModifiedDate = DateTime.Now;
                        }
                    }

                    efSource.CompanyId = mSource.CompanyId;
                    efSource.Name = mSource.Name;
                    efSource.IsActive = mSource.IsActive;

                    context.SaveChanges();
                    mSource.Id = efSource.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mSource;
        }

        public Domain.Source Get(int id)
        {
            var mSource = new Domain.Source();
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efSource = (from s in context.Sources
                                  where s.Id == id
                                  select new
                                  {
                                      Id = s.Id,
                                      CompanyId = s.CompanyId,
                                      Name = s.Name,
                                      IsActive = s.IsActive,
                                  }).FirstOrDefault();

                    if (efSource != null)
                    {
                        mSource = JsonSerializer.Deserialize<Domain.Source>(JsonSerializer.Serialize(efSource));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mSource;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efSource = context.Sources.FirstOrDefault(x => x.Id == id);
                    if (efSource != null)
                    {
                        var audit = context.AuditTrails.Find(efSource.AuditId);
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

        public List<Domain.Source> GetActiveSource()
        {
            var mSource = new List<Domain.Source>();
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efSource = from s in context.Sources
                                   join at in context.AuditTrails on s.AuditId equals at.Id
                                   where at.DeletedBy == null && at.DeletedDate == null
                                   select new
                                      {
                                          Id = s.Id,
                                          Name = s.Name,
                                          AuditId = s.AuditId
                                      };

                    mSource = JsonSerializer.Deserialize<List<Domain.Source>>(JsonSerializer.Serialize(efSource.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mSource;
        }
    }
}
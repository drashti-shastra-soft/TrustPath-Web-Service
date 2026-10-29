using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using TrustPathWebService.Infrastructure.IRepository;
using TrustPathWebService.Infrastructure.Utils;

namespace TrustPathWebService.Repository
{
    public class UserRepository : IUserRepository
    {
        public Domain.UserLister GetAll(Domain.UserLister mLister)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var query = from u in context.Users
                                join c in context.Companies on u.CompanyId equals c.Id into joinUC
                                from c in joinUC.DefaultIfEmpty()
                                join role in context.Roles on u.RoleId equals role.Id
                                join at in context.AuditTrails on u.AuditId equals at.Id
                                where at.DeletedBy == null && at.DeletedDate == null
                                select new
                                {
                                    Id = u.Id,
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    EmailAddress = u.EmailAddress,
                                    Password = u.Password,
                                    MobileNumber = u.MobileNumber,
                                    IsActive = u.IsActive,
                                    RoleId = u.RoleId,
                                    CompanyId = u.CompanyId,
                                    Role = role.Name,
                                    Company = c.Name,
                                };

                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {

                        var criteria = mLister.SearchCriteria;

                        if (criteria.FirstName.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.FirstName) && x.FirstName.ToUpper().Contains(criteria.FirstName.ToUpper()));

                        if (criteria.LastName.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.LastName) && x.LastName.ToUpper().Contains(criteria.LastName.ToUpper()));

                        if (criteria.EmailAddress.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.EmailAddress) && x.EmailAddress.ToUpper().Contains(criteria.EmailAddress.ToUpper()));

                        if (criteria.MobileNumber.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.MobileNumber) && x.MobileNumber.ToUpper().Contains(criteria.MobileNumber.ToUpper()));

                        if (criteria.RoleId > 0)
                            query = query.Where(x => x.RoleId == criteria.RoleId);

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
                    var efUsers = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efUsers.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efUsers = efUsers.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.User>>(JsonSerializer.Serialize(efUsers.ToList()));
                }
            }
            catch
            {
                throw;
            }

            return mLister;

        }

        public Domain.User Upsert(Domain.User mUser)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efUser = context.Users.FirstOrDefault(x => x.Id == mUser.Id);

                    if (efUser == null)
                    {
                        var audit = new Persistance.AuditTrail
                        {
                            CreatedBy = mUser.CreatedBy,
                            CreatedDate = DateTime.Now
                        };

                        context.AuditTrails.Add(audit);
                        context.SaveChanges();

                        efUser = new Persistance.User
                        {
                            Password = mUser.Password,
                            AuditId = audit.Id
                        };

                        context.Users.Add(efUser);
                    }
                    else
                    {
                        var audit = context.AuditTrails.Find(efUser.AuditId);
                        if (audit != null)
                        {
                            audit.LastModifiedBy = mUser.LastModifiedBy;
                            audit.LastModifiedDate = DateTime.Now;
                        }
                    }
                    efUser.CompanyId = mUser.CompanyId;
                    efUser.RoleId = mUser.RoleId;
                    efUser.FirstName = mUser.FirstName;
                    efUser.LastName = mUser.LastName;
                    efUser.EmailAddress = mUser.EmailAddress;
                    efUser.MobileNumber = mUser.MobileNumber;
                    efUser.IsActive = mUser.IsActive;
                    context.SaveChanges();
                    mUser.Id = efUser.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mUser;
        }

        public Domain.User Get(int id)
        {
            var mUser = new Domain.User();
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efUser = (from u in context.Users
                                  where u.Id == id
                                  select new
                                  {
                                      Id = u.Id,
                                      FirstName = u.FirstName,
                                      LastName = u.LastName,
                                      EmailAddress = u.EmailAddress,
                                      MobileNumber = u.MobileNumber,
                                      IsActive = u.IsActive,
                                  }).FirstOrDefault();

                    if (efUser != null)
                    {
                        mUser = JsonSerializer.Deserialize<Domain.User>(JsonSerializer.Serialize(efUser));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mUser;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efUser = context.Users.FirstOrDefault(x => x.Id == id);
                    if (efUser != null)
                    {
                        var audit = context.AuditTrails.Find(efUser.AuditId);
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

        public bool CheckEmailAddressExist(Domain.User mUser)
        {
            bool isExist = false;
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    isExist = context.Users.Where(x => x.Id != mUser.Id && x.EmailAddress.Trim().ToUpper() == mUser.EmailAddress.Trim().ToUpper()).Count() > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return isExist;
        }

        public Domain.User GetBy(string emailAddress)
        {
            var mUser = new Domain.User();
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efUser = (from u in context.Users
                                  join at in context.AuditTrails on u.AuditId equals at.Id
                                  where u.EmailAddress.Trim().ToUpper() == emailAddress.Trim().ToUpper() && at.DeletedBy == null && at.DeletedDate == null
                                  select new
                                  {
                                      Id = u.Id,
                                      FirstName = u.FirstName,
                                      LastName = u.LastName,
                                      EmailAddress = u.EmailAddress,
                                      Password = u.Password,
                                      MobileNumber = u.MobileNumber,
                                      IsActive = u.IsActive,
                                      RoleId = u.RoleId,
                                      CompanyId = u.CompanyId
                                  }).AsEnumerable();

                    mUser = JsonSerializer.Deserialize<Domain.User>(JsonSerializer.Serialize(efUser.FirstOrDefault()));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mUser;
        }

        public string GetPasswordBy(int id)
        {
            string password = string.Empty;
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    password = context.Users.Where(x => x.Id == id).Select(s => s.Password).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return password;
        }

        public void SetPasswordBy(int id, string password)
        {
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efUser = context.Users.FirstOrDefault(x => x.Id == id);
                    if (efUser != null && efUser.Id > default(int))
                    {
                        efUser.Password = password;
                        var audit = context.AuditTrails.Find(efUser.AuditId);
                        if (audit != null)
                        {
                            audit.LastModifiedBy = id;
                            audit.LastModifiedDate = DateTime.Now;
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

        public List<Domain.User> GetActiveUsers(int companyId)
        {
            var userList = new List<Domain.User>();
            try
            {
                using (var context = new Persistance.TrustPathDBEntities())
                {
                    var efUsers = from user in context.Users
                                  join at in context.AuditTrails on user.AuditId equals at.Id
                                  where at.DeletedBy == null && at.DeletedDate == null && user.CompanyId == companyId
                                  select new
                                  {
                                      Id = user.Id,
                                      FirstName = user.FirstName,
                                      LastName = user.LastName,
                                  };

                    var json = JsonSerializer.Serialize(efUsers.ToList());
                    userList = JsonSerializer.Deserialize<List<Domain.User>>(json);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return userList;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPathWebService.Infrastructure.IRepository
{
    public interface IUserRepository
    {
        Domain.UserLister GetAll(Domain.UserLister mLister);

        Domain.User Upsert(Domain.User mUser);

        Domain.User Get(int id);

        void Delete(int id, int loginUserId);
        bool CheckEmailAddressExist(Domain.User mUser);
        Domain.User GetBy(string emailAddress);
        string GetPasswordBy(int id);
        void SetPasswordBy(int id, string password);
        List<Domain.User> GetActiveUsers(int companyId);
    }
}

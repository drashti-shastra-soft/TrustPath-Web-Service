using System;
using System.Linq;

namespace TrustPathWebService.Infrastructure.IService
{
    public interface IChangePasswordService
    {
        void Upsert(Domain.ChangePassword mChangePassword);
    }
}

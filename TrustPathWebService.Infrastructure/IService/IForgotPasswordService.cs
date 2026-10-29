using System;
using System.Linq;

namespace TrustPathWebService.Infrastructure.IService
{
    public interface IForgotPasswordService
    {
        void Initiate(Domain.ForgotPassword mForgotPassword);
    }
}

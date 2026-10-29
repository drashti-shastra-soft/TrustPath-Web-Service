using System.Web.Http;
using TrustPathWebService.Infrastructure.IRepository;
using TrustPathWebService.Infrastructure.IService;
using TrustPathWebService.Repository;
using TrustPathWebService.Service;
using Unity;
using Unity.WebApi;

namespace TrustPathWebService
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            //Service
            container.RegisterType<IAuthenticationService, AuthenticationService>();
            container.RegisterType<IBusinessTypeService, BusinessTypeService>();
            container.RegisterType<IChangePasswordService, ChangePasswordService>();
            container.RegisterType<ICompanyService, CompanyService>();
            container.RegisterType<IEmailSenderService, EmailSenderService>();
            container.RegisterType<IFileReaderService, FileReaderService>();
            container.RegisterType<IForgotPasswordService, ForgotPasswordService>();
            container.RegisterType<IJWTokenService, JWTokenService>();
            container.RegisterType<ILeadCallService, LeadCallService>();
            container.RegisterType<ILeadService, LeadService>();
            container.RegisterType<IRoleService, RoleService>();
            container.RegisterType<ISourceService, SourceService>();
            container.RegisterType<IStateService, StateService>();
            container.RegisterType<IUserService, UserService>();

            //Repository
            container.RegisterType<IBusinessTypeRepository, BusinessTypeRepository>();
            container.RegisterType<ICompanyRepository, CompanyRepository>();
            container.RegisterType<ILeadCallRepository, LeadCallRepository>();
            container.RegisterType<ILeadRepository, LeadRepository>();
            container.RegisterType<IRoleRepository, RoleRepository>();
            container.RegisterType<ISourceRepository, SourceRepository>();
            container.RegisterType<IStateRepository, StateRepository>();
            container.RegisterType<IUserRepository, UserRepository>();

            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}
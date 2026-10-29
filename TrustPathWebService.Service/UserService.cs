using System;
using System.Collections.Generic;
using System.Configuration;
using TrustPathWebService.Infrastructure.CustomException;
using TrustPathWebService.Infrastructure.IRepository;
using TrustPathWebService.Infrastructure.IService;
using TrustPathWebService.Infrastructure.Utils;

namespace TrustPathWebService.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IFileReaderService _fileReaderService;
        private readonly IEmailSenderService _emailSenderService;
        private readonly ICompanyRepository _companyRepo;

        public UserService(IUserRepository repo, ICompanyRepository companyRepo, IFileReaderService fileReaderService, IEmailSenderService emailSenderService)
        {
            _repo = repo;
            _fileReaderService = fileReaderService;
            _emailSenderService = emailSenderService;
            _companyRepo = companyRepo;
        }

        public Domain.UserLister GetAll(Domain.UserLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.User Upsert(Domain.User mUser)
        {
            if (mUser == null)
                throw new APIRequestFailedException("User object must not be null.");

            SetUserPassword(mUser);

            var mCreatedUser = _repo.Upsert(mUser);
            if (IsValidUser(mCreatedUser))
            {
                var mCompany = _companyRepo.Get(mCreatedUser.CompanyId);
                SendEmailOnCreateUser(mCreatedUser, mCompany);
            }

            return mCreatedUser;
        }


        public Domain.User Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

        public bool CheckEmailAddressExist(Domain.User mUser)
        {
            return _repo.CheckEmailAddressExist(mUser);
        }

        public List<Domain.User> GetActiveUsers(int CompanyId)
        {
            return _repo.GetActiveUsers(CompanyId);
        }

        #region Private Method

        private void SendEmailOnCreateUser(Domain.User mUser, Domain.Company mCompany)
        {
            string content = _fileReaderService.ReadFileContent("~/EmailTemplates/SendCredentialsOnCreateUser.html");
            string finalContent = PopulateEmailTemplate(content, mUser, mCompany);
            _emailSenderService.SendEmailAsync($"Welcome to {mCompany.Name} – Your Account Details Inside", finalContent, mUser.EmailAddress);
        }

        private string PopulateEmailTemplate(string template, Domain.User mUser, Domain.Company mCompany)
        {
            var replacements = new Dictionary<string, string>
            {
                { "{{CompanyName}}", mCompany.Name },
                { "{{FirstName}}", mUser.FirstName },
                { "{{Username}}", mUser.EmailAddress },
                { "{{Password}}", mUser.PlainTextPassword},
                { "{{Year}}", DateTime.Now.Year.ToString() }
            };

            foreach (var pair in replacements)
            {
                template = template.Replace(pair.Key, pair.Value);
            }

            return template;
        }



        private void SetUserPassword(Domain.User user)
        {
            user.PlainTextPassword = StaticMethods.GeneratePassword(8);
            var salt = ConfigurationManager.AppSettings.Get("Salt");
            user.Password = PasswordEncryptor.Encrypt(user.PlainTextPassword, salt);
        }

        private bool IsValidUser(Domain.User savedUser)
        {
            return savedUser != null && savedUser.Id > 0;
        }

        #endregion
    }
}

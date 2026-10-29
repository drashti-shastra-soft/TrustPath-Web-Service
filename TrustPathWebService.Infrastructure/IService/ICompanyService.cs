using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPathWebService.Infrastructure.IService
{
    public interface ICompanyService
    {
        Domain.CompanyLister GetAll(Domain.CompanyLister mLister);
        List<Domain.Company> GetActiveCompanies();
        Domain.Company Upsert(Domain.Company mCompany);
        Domain.Company Get(int id);
        void Delete(int id, int loginUserId);
    }
}

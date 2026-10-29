using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPathWebService.Infrastructure.IRepository
{
    public interface IBusinessTypeRepository
    {
        Domain.BusinessTypeLister GetAll(Domain.BusinessTypeLister mLister);
        Domain.BusinessType Upsert(Domain.BusinessType mBusinessType);
        Domain.BusinessType Get(int id);
        void Delete(int id, int loginUserId);
        List<Domain.BusinessType> GetActiveBusinessType();
    }
}

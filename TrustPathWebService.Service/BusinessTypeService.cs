using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustPathWebService.Infrastructure.IRepository;
using TrustPathWebService.Infrastructure.IService;

namespace TrustPathWebService.Service
{
    public class BusinessTypeService : IBusinessTypeService
    {
        private readonly IBusinessTypeRepository _repo;

        public BusinessTypeService(IBusinessTypeRepository repo)
        {
            _repo = repo;
        }

        public Domain.BusinessTypeLister GetAll(Domain.BusinessTypeLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.BusinessType Upsert(Domain.BusinessType mBusinessType)
        {
            return _repo.Upsert(mBusinessType);
        }

        public Domain.BusinessType Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

        public List<Domain.BusinessType> GetActiveBusiness()
        {
             return _repo.GetActiveBusinessType();
        }
    }
}

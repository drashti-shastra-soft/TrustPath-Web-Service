using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustPathWebService.Infrastructure.IRepository;
using TrustPathWebService.Infrastructure.IService;

namespace TrustPathWebService.Service
{
    public class StateService : IStateService
    {
        private readonly IStateRepository _repo;

        public StateService(IStateRepository repo)
        {
            _repo = repo;
        }

        public Domain.StateLister GetAll(Domain.StateLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.State Upsert(Domain.State mState)
        {
            return _repo.Upsert(mState);
        }

        public Domain.State Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

        public List<Domain.State> GetActiveState()
        {
            return _repo.GetActiveState();
        }

    }
}

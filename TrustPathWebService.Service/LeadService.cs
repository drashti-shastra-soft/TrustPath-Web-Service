using TrustPathWebService.Infrastructure.IRepository;
using TrustPathWebService.Infrastructure.IService;

namespace TrustPathWebService.Service
{
    public class LeadService : ILeadService
    {
        private readonly ILeadRepository _repo;
        private readonly ILeadCallRepository _leadcallrepo;

        public LeadService(ILeadRepository repo, ILeadCallRepository leadcallrepo)
        {
            _repo = repo;
            _leadcallrepo = leadcallrepo;
        }

        public Domain.LeadLister GetAll(Domain.LeadLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.Lead Upsert(Domain.Lead mLead)
        {
            return _repo.Upsert(mLead);
        }

        public Domain.Lead Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }
    }
}

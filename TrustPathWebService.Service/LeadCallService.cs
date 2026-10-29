using TrustPathWebService.Infrastructure.IRepository;
using TrustPathWebService.Infrastructure.IService;
using System.Collections.Generic;

namespace TrustPathWebService.Service
{
    public class LeadCallService : ILeadCallService
    {
        private readonly ILeadCallRepository _repo;

        public LeadCallService(ILeadCallRepository repo)
        {
            _repo = repo;
        }

        public Domain.LeadCallLister GetAll(Domain.LeadCallLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.LeadCall Upsert(Domain.LeadCall mLeadCall)
        {
            return _repo.Upsert(mLeadCall);
        }

        public Domain.LeadCall Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

        public Domain.LeadCall WhatsAppMessage(Domain.LeadCall mLeadCall)
        {
           return _repo.WhatsAppMessage(mLeadCall);
        }
    }
}

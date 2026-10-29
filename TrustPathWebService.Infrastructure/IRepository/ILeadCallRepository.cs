using System;
using System.Collections.Generic;
using System.Linq;

namespace TrustPathWebService.Infrastructure.IRepository
{
    public interface ILeadCallRepository
    {
        Domain.LeadCallLister GetAll(Domain.LeadCallLister mLister);

        Domain.LeadCall Upsert(Domain.LeadCall mLeadcall);
        Domain.LeadCall Get(int id);
        Domain.LeadCall WhatsAppMessage(Domain.LeadCall mLeadCall);
        void Delete(int id, int loginUserId);
    }
}

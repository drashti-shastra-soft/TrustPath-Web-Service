using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPathWebService.Infrastructure.IService
{
    public interface ILeadCallService
    {   
        Domain.LeadCallLister GetAll(Domain.LeadCallLister mLister);

        Domain.LeadCall Upsert(Domain.LeadCall mLeadCall);

        Domain.LeadCall Get(int id);

        Domain.LeadCall WhatsAppMessage(Domain.LeadCall mLeadCall);

        void Delete(int id, int loginUserId);
    }
}

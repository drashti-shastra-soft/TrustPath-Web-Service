using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPathWebService.Infrastructure.IService
{
    public interface ILeadService
    {
        Domain.LeadLister GetAll(Domain.LeadLister mLister);
        Domain.Lead Upsert(Domain.Lead mLead);
        Domain.Lead Get(int id);
        void Delete(int id, int loginUserId);
    }
}

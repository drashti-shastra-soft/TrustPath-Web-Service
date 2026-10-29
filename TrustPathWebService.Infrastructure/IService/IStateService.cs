using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPathWebService.Infrastructure.IService
{
    public interface IStateService
    {
        Domain.StateLister GetAll(Domain.StateLister mLister);

        Domain.State Upsert(Domain.State mState);

        Domain.State Get(int id);

        void Delete(int id, int loginUserId);

        List<Domain.State> GetActiveState();
    }
}

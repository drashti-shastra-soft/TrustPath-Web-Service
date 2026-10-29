using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPathWebService.Infrastructure.IService
{
    public interface ISourceService
    {
        Domain.SourceLister GetAll(Domain.SourceLister mLister);
        Domain.Source Upsert(Domain.Source mSource);
        Domain.Source Get(int id);
        void Delete(int id, int loginUserId);

        List<Domain.Source> GetActiveSource();
    }
}

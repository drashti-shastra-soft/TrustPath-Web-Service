using System.Collections.Generic;
using TrustPathWebService.Infrastructure.IRepository;
using TrustPathWebService.Infrastructure.IService;

namespace TrustPathWebService.Service
{
    public class SourceService : ISourceService
    {
        private readonly ISourceRepository _repo;

        public SourceService(ISourceRepository repo)
        {
            _repo = repo;
        }

        public Domain.SourceLister GetAll(Domain.SourceLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.Source Upsert(Domain.Source mSource)  
        {
            return _repo.Upsert(mSource);
        }

        public Domain.Source Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

        public List<Domain.Source> GetActiveSource()
        {
            return _repo.GetActiveSource();
        }
    }
}

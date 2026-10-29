using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustPathWebService.Infrastructure.IRepository;
using TrustPathWebService.Infrastructure.IService;

namespace TrustPathWebService.Service
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repo;
        public RoleService(IRoleRepository repo)
        {
            _repo = repo;
        }

        public List<Domain.Role> GetAll()
        {
            return _repo.GetAll();
        }
    }
}

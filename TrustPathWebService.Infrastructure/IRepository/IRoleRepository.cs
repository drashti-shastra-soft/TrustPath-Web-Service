using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPathWebService.Infrastructure.IRepository
{
    public interface IRoleRepository
    {
        List<Domain.Role> GetAll();
    }
}

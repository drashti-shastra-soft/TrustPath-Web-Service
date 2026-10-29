using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPathWebService.Infrastructure.IService
{
    public interface IRoleService
    {
        List<Domain.Role> GetAll();
    }
}

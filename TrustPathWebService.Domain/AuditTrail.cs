using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPathWebService.Domain
{
    public abstract class AuditTrail
    {
        public int CreatedBy { get; set; }
        public int? LastModifiedBy { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPathWebService.Domain
{
    public class State : AuditTrail
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public int AuditId { get; set; }
    }
}

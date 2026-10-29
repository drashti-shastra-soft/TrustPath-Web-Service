using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPathWebService.Domain
{
    public class LeadCallLister
    {
        public List<LeadCall> List { get; set; } = new List<LeadCall>();

        public LeadCall SearchCriteria { get; set; } = new LeadCall();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}

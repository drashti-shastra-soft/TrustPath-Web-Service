using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPathWebService.Domain
{
    public class LeadLister
    {
        public List<Lead> List { get; set; } = new List<Lead>();

        public Lead SearchCriteria { get; set; } = new Lead();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}

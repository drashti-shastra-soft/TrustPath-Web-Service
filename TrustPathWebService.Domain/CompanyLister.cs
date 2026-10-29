using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPathWebService.Domain
{
    public class CompanyLister
    {
        public List<Company> List { get; set; } = new List<Company>();

        public Company SearchCriteria { get; set; } = new Company();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}

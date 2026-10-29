using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPathWebService.Domain
{
    public class BusinessTypeLister
    {
        public List<BusinessType> List { get; set; } = new List<BusinessType>();

        public BusinessType SearchCriteria { get; set; } = new BusinessType();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}

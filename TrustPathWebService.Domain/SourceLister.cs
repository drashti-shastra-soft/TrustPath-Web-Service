using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustPathWebService.Domain
{
    public class SourceLister
    {
        public List<Source> List { get; set; } = new List<Source>();

        public Source SearchCriteria { get; set; } = new Source();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}

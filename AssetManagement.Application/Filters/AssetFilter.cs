using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Application.Filters
{
    public class AssetFilter
    {
        public string search { get; set; }
        public string category { get; set; }
        public string state { get; set; }
        public string sorted { get; set; }  
        public string order { get; set; }
    }
}

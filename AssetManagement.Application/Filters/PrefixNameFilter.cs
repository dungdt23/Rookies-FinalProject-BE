using AssetManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Application.Filters
{
    public class PrefixNameFilter
    {
        public bool isPrefix { get; set; }
        public string value { get; set; }
    }
}

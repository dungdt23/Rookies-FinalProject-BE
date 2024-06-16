using AssetManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Application.Filters
{
    public class AssetFilter
    {
        public Guid locationId { get; set; }
        public TypeAssetState? state { get; set; }
        public string? category { get; set; }
        public string? search { get; set; }  
        public AssetSort sort { get; set; } = AssetSort.AssetCode;
        public TypeOrder order { get; set; } = TypeOrder.Ascending;
    }
    public enum AssetSort
    {
        AssetCode = 1,
        AssetName = 2,  
        Category = 3,
        State = 4
    }

}

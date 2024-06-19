using AssetManagement.Domain.Enums;

namespace AssetManagement.Application.Filters
{
    public class AssetFilter
    {
        public TypeAssetState? state { get; set; }
        public Guid? category { get; set; }
        public string? search { get; set; }  
        public AssetSort sort { get; set; } = AssetSort.AssetCode;
        public TypeOrder order { get; set; } = TypeOrder.Ascending;
    }
    public enum AssetSort
    {
        AssetCode = 1,
        AssetName = 2,  
        CategoryName = 3,
        State = 4
    }

}

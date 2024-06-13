using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;

namespace AssetManagement.Application.Filters
{
    public class UserFilter
    {
        public Guid? TypeId { get; set; }
        public string? SearchString { get; set; }
        public bool IsAscending { get; set; }  = true;
        public int FieldFilter {  get; set; }  =  UserFilterConstant.StaffCodeFilter;
    }
}

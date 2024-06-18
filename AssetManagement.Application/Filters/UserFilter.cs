namespace AssetManagement.Application.Filters
{
    public class UserFilter
    {
        public Guid locationId { get; set; }
        public UserType? UserType { get; set; }
        public string? SearchString { get; set; }
        public bool IsAscending { get; set; }  = true;
        public FieldType FieldFilter {  get; set; } = FieldType.StaffCode;
    }

    public enum UserType
    {
        Admin = 0,
        Staff = 1
    }
    public enum FieldType
    {
        StaffCode = 1,
        FullName = 2,
        JoinedDate = 3,
        Type = 4
    }

}

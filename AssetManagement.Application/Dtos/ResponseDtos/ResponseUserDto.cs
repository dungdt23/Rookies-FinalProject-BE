using AssetManagement.Domain.Enums;

namespace AssetManagement.Application.Dtos.ResponseDtos
{
    public class ResponseUserDto
    {
        public Guid Id { get; set; }
        public string StaffCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public DateTime JoinedDate { get; set; }
        public TypeGender TypeGender { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
    }
}

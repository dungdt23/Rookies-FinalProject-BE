using AssetManagement.Domain.Enums;

namespace AssetManagement.Application.Dtos.ResponseDtos
{
    public class ResponseReturnRequestDto
    {
        public Guid Id { get; set; }
        public Guid RequestorId { get; set; }
        public Guid? ResponderId { get; set; }
        public Guid AssignmentId { get; set; }
        public TypeRequestState State { get; set; }
        public DateTime? ReturnedDate { get; set; }
    }
}

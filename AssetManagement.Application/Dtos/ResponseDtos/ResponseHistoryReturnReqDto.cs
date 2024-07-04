using AssetManagement.Domain.Enums;

namespace AssetManagement.Application.Dtos.ResponseDtos
{
    public class ResponseHistoryReturnRequestDto
    {
        public Guid Id { get; set; }
        public Guid AssignmentId { get; set; }
        public DateTime AssignmentAssignedDate { get; set; }

        public TypeRequestState State { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string AssetCode { get; set; }
        public string AssetName { get; set; }
    }
}

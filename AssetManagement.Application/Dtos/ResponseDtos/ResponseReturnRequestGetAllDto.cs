using AssetManagement.Domain.Enums;

namespace AssetManagement.Application.Dtos.ResponseDtos
{
    public class ResponseReturnRequestGetAllDto
    {
        public Guid Id { get; set; }

        public Guid RequestorId { get; set; }
        public string RequestorUsername { get; set; }

        public Guid? ResponderId { get; set; }
        public string? ResponderUsername { get; set; }

        public Guid AssignmentId { get; set; }
        public DateTime AssignmentAssignedDate { get; set; }
        public Guid AssetId { get; set; }
        public string AssetCode { get; set; }
        public string AssetName { get; set; }

        public TypeRequestState State { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
    }
}

using AssetManagement.Domain.Common;
using AssetManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Domain.Entities
{
    public class ReturnRequest : BaseEntity
    {
        [Key]
        public override Guid Id { get; set; } = Guid.NewGuid();
        public TypeRequestState State { get; set; } = TypeRequestState.WaitingForReturning;
        public DateTime RequestedDate { get; set; } = DateTime.Now;
        public DateTime? ReturnedDate { get; set; }
        [Required]
        public Guid RequestorId { get; set; }
        public Guid? ResponderId { get; set; }
        [Required]
        public Guid AssignmentId { get; set; }
        [Required]
        public Guid LocationId { get; set; }

        // Navigation Properties
        public User Requestor { get; set; }
        public User? Responder { get; set; }
        public Assignment Assignment { get; set; }
        public Location Location { get; set; }
    }
}

using AssetManagement.Domain.Common;
using AssetManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Domain.Entities
{
    public class Assignment : BaseEntity
    {
        [Key]
        public override Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid AssetId { get; set; }
        [Required]
        public Guid AssignerId { get; set; }
        [Required]
        public Guid AssigneeId { get; set; }
        public TypeAssignmentState State { get; set; }
        [Required]
        public DateTime AssignedDate { get; set; }
        [MaxLength(500)]
        public string? Note { get; set; }

        public User Assigner { get; set; }
        public User Assignee { get; set; }
        public Asset Asset { get; set; }
        public ICollection<ReturnRequest> ReturnRequests { get; set; }
    }
}

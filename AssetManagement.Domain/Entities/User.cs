using AssetManagement.Domain.Common;
using AssetManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        [Key]
        public override Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public  string StaffCode { get; set; }
        [Required]
        [MaxLength(150)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(150)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(150)]
        public string UserName { get; set; }
        public string? Email { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; } = new byte[32];
        [Required]
        public byte[] PasswordSalt { get; set; } = new byte[32];
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]

        public DateTime JoinedDate { get; set; }
        [Required]

        public TypeGender Gender { get; set; }
        [Required]

        public Guid LocationId { get; set; }
        [Required]

        public Guid TypeId { get; set; }

        public Location Location { get; set; }
        public Type Type { get; set; }

        public ICollection<Assignment> AssignedAssignments { get; set; } 
        public ICollection<Assignment> ReceivedAssignments { get; set; } 
        public ICollection<ReturnRequest> RequestedReturnRequests { get; set; }
        public ICollection<ReturnRequest> RespondedReturnRequests { get; set; }
    }
}

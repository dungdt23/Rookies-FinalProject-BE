using AssetManagement.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Domain.Entities
{
    public class Location : BaseEntity
    {
        [Key]
        public override Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(100)]
        public required string LocationName { get; set; }
        public ICollection<User> Users { get; set; }
    }
}

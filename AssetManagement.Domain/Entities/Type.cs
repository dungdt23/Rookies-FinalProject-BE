using AssetManagement.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Domain.Entities
{
    public class Type : BaseEntity
    {
        [Key]
        public override Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(100)]
        public required string TypeName { get; set; }
        [Required]
        [MaxLength(150)]
        public required string Description { get; set; }

        public ICollection<User> Users { get; set; }
        
    }
}

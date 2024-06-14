using AssetManagement.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Domain.Entities
{
    public class Type : BaseEntity
    {
        [Key]
        public override Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(200)]
        public  string TypeName { get; set; }
        [Required]
        [MaxLength(200)]
        public  string Description { get; set; }

        public ICollection<User> Users { get; set; }
        
    }
}

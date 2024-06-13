using AssetManagement.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Domain.Entities
{
    public class Category : BaseEntity
    {
        [Key]
        public override Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public required string Prefix { get; set; }
        [Required]
        [MaxLength(100)]
        public required string CategoryName { get; set; }

        public ICollection<Asset> Assets { get; set; }
    }
}

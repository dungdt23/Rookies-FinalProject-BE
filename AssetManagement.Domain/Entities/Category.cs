using AssetManagement.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Domain.Entities
{
    public class Category : BaseEntity
    {
        [Key]
        public override Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(4)]
        [MinLength(2)]
        public string Prefix { get; set; }
        [Required]
        [MaxLength(200)]
        public string CategoryName { get; set; }

        public ICollection<Asset> Assets { get; set; }
    }
}

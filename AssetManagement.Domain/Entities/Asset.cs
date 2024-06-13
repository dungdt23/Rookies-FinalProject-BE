using AssetManagement.Domain.Common;
using AssetManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Domain.Entities
{
    public class Asset : BaseEntity
    {
        [Key]
        public override Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string AssetCode { get; set; }
        [Required]
        [MaxLength(100)]
        public string AssetName { get; set; }
        [Required]
        [MaxLength(4)]
        [MinLength(2)]
        public string Prefix {get; set; }   
        [Required]
        public Guid CategoryId { get; set; }
        [Required]
        public string Specification { get; set; }
        [Required]
        public DateTime InstalledDate {  get; set; }
        [Required]
        public TypeAssetState State { get; set; }

        public Category Category { get; set; }
        public ICollection<Assignment> Assignments { get; set; }
        public ICollection<ReturnRequest> ReturnRequests { get; set; }
    }
}

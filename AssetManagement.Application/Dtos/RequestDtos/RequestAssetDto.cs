using AssetManagement.Application.Validation;
using AssetManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Application.Dtos.RequestDtos
{
    public class RequestAssetDto
    {
        [Required]
        [AssetNaming]
        public string AssetName { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        public Guid? LocationId { get; set; }
        [Required]
        [Specification]
        public string Specification { get; set; }
        [Required]
        [InstalledDate]
        public DateTime InstalledDate { get; set; }
        [Required]
        public TypeAssetState State { get; set; }
    }
}

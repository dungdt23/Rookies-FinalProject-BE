using AssetManagement.Application.Validation;
using AssetManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Application.Dtos.RequestDtos
{
    public class RequestAssetDto
    {
        [Required]
        [Naming]
        public string AssetName { get; set; }
        public string AssetCode { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        [Required]
        public Guid LocationId { get; set; }
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

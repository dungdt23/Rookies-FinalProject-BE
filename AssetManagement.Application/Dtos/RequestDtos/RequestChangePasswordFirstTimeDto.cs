using AssetManagement.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Application.Dtos.RequestDtos
{
    public class RequestChangePasswordFirstTimeDto
    {
        [Required]
        [Password]
        public string NewPassword { get; set; }
    }
}

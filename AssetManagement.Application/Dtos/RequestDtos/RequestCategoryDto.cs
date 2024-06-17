using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Application.Dtos.RequestDtos
{
    public class RequestCategoryDto
    {
        [Required]
        [MaxLength(4)]
        [MinLength(2)]
        public string Prefix { get; set; }
        [Required]
        [MaxLength(200)]
        public string CategoryName { get; set; }

    }
}

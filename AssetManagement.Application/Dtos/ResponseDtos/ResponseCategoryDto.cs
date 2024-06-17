using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Application.Dtos.ResponseDtos
{
    public class ResponseCategoryDto
    {
        public Guid Id { get; set; }
        public string Prefix { get; set; }
        public string CategoryName { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Application.Dtos.Common
{
    public class ResponseMessageDto
    {
        [Required]
        public string Message { get; set; }

        public ResponseMessageDto(string message)
        {
            Message = message;
        }
    }
}

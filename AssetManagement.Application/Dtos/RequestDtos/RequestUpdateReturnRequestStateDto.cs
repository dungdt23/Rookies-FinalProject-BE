using AssetManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Application.Dtos.RequestDtos
{
    public class RequestUpdateReturnRequestStateDto
    {
        [Required]
        public TypeRequestState State { get; set; }
    }
}

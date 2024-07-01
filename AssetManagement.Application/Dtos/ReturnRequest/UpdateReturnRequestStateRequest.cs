using AssetManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Application.Dtos.ReturnRequest
{
    public class UpdateReturnRequestStateRequest
    {
        [Required]
        public TypeRequestState State { get; set; }
    }
}

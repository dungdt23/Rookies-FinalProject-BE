using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AssetManagement.Application.Dtos.RequestDtos
{
    public class RequestAssignmentRespondDto
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        [Required]
        public Guid AssignmentId { get; set; }
        [Required]
        public bool IsAccept {get; set; } = false;
    }
}


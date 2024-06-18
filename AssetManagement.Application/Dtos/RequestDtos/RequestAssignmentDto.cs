using AssetManagement.Application.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AssetManagement.Application.Dtos.RequestDtos
{
	public class RequestAssignmentDto
	{
		[Required]
		public Guid AssetId { get; set; }
		[Required]
		public Guid AssignerId { get; set; }
		[JsonIgnore]
		public Guid AssigneeId { get; set; }
		[Required]
		[LaterThanCurrentTime]
		public DateTime AssignedDate { get; set; }
		[MaxLength(500)]
		public string? Note { get; set; }
	}
}

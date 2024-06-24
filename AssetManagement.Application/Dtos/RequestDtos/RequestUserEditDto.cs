using AssetManagement.Application.Validation;
using AssetManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AssetManagement.Application.Dtos.RequestDtos
{
	public class RequestUserEditDto
	{
		[Required]
		[WorkingAge]
		public DateTime DateOfBirth { get; set; }

		[Required]
		public TypeGender Gender { get; set; } = TypeGender.Female;

		[Required]
		[WorkingDay]
		[LaterThanDateOfBirth(nameof(DateOfBirth))]
		public DateTime JoinedDate { get; set; }

		[Required]
		public string Type { get; set; }

	}
}

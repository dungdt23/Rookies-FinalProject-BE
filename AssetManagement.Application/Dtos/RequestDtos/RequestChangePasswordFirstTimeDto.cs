using AssetManagement.Application.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Application.Dtos.RequestDtos
{
	public class RequestChangePasswordFirstTimeDto
	{
		[Required]
		[Password]
		public string Password { get; set; } = null!;
	}
}

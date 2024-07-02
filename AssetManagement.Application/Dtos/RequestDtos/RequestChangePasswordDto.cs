using AssetManagement.Application.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Application.Dtos.RequestDtos
{
	public class RequestChangePasswordDto
	{
		[Required]
		[Password]
		public string OldPassword { get; set; } = null!;
		[Required]
		[Password]
		[NotEqualTo("OldPassword", ErrorMessage = "New password should not be the same as old password")]
		public string NewPassword { get; set; } = null!;
	}
}

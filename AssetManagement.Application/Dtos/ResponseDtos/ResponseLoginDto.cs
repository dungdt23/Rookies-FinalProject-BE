using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Application.Dtos.ResponseDtos
{
	public class ResponseLoginDto
	{
		public string TokenType { get; set; } = "Bearer";
		public string Token { get; set; }
		public bool IsFirstTimeLogin {  get; set; }
	}
}

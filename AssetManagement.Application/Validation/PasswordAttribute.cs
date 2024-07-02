using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Application.Validation
{
	public class PasswordAttribute : ValidationAttribute
	{
		private const int MIN_LENGTH = 8;
		private const int MAX_LENGTH = 20;
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			string stringValue = value.ToString();

			if (stringValue.Length < MIN_LENGTH || stringValue.Length > MAX_LENGTH)
			{
				return new ValidationResult($"The name must be {MIN_LENGTH}-{MAX_LENGTH} characters long.");
			}

			return ValidationResult.Success;
		}
	}
}

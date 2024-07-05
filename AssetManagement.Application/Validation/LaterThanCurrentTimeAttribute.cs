using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Application.Validation
{
	public class LaterThanCurrentTimeAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			DateTime dateValue;
			if (!DateTime.TryParse(value.ToString(), out dateValue))
			{
				return new ValidationResult("Incorrect date format");
			}
			if (dateValue.Date < DateTime.UtcNow.Date)
			{
				return new ValidationResult("Inputted date must be later than current time");
			}
			return ValidationResult.Success;
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Application.Validation
{
	public class NotEqualToAttribute : ValidationAttribute
	{
		private readonly string _comparisonProperty;

		public NotEqualToAttribute(string comparisonProperty)
		{
			_comparisonProperty = comparisonProperty;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var currentValue = value as string;

			var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

			if (property == null)
				throw new ArgumentException("Property with this name not found");

			var comparisonValue = property.GetValue(validationContext.ObjectInstance) as string;

			if (currentValue == comparisonValue)
				return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} should not be the same as {_comparisonProperty}");

			return ValidationResult.Success;
		}
	}
}

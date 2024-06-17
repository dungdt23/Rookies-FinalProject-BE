using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AssetManagement.Application.Validation
{
    public class SpecificationAttribute : ValidationAttribute
    {
        private const int MIN_LENGTH = 0;
        private const int MAX_LENGTH = 500;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string stringValue = value.ToString();

            if (stringValue.Length < MIN_LENGTH || stringValue.Length > MAX_LENGTH)
            {
                return new ValidationResult($"The specification must be {MIN_LENGTH}-{MAX_LENGTH} characters long.");
            }
            return ValidationResult.Success;
        }
    }
}

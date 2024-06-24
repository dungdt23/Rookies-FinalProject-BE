using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Application.Validation
{
    public class AssetNamingAttribute : ValidationAttribute
    {
        private const int MIN_LENGTH = 2;
        private const int MAX_LENGTH = 200;
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

using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Application.Validation
{
    public class InstalledDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dateValue;
            if (!DateTime.TryParse(value.ToString(), out dateValue))
            {
                return new ValidationResult("Incorrect date format");
            }
            if (dateValue > DateTime.UtcNow)
            {
                return new ValidationResult("Installed date must be in the past");
            }
            return ValidationResult.Success;
        }
    }
}

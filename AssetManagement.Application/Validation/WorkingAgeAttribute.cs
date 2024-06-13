using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Application.Validation;

public class WorkingAgeAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        DateTime dateValue; 
        if (!DateTime.TryParse(value.ToString(), out dateValue))
        {
            return new ValidationResult("Incorrect date format");
        }
        var today = DateTime.Today;
        var age = today.Year - dateValue.Year;

        if (dateValue.Date > today.AddYears(-age))
        {
            age -= 1;
        }

        if (age < 18)
        {
            return new ValidationResult("Date of birth imply user is less than 18 years old");
        }


        return ValidationResult.Success;
    }
}

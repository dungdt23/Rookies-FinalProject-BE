using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Application.Validation;

public class WorkingDayAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        DateTime dateValue;
        if (!DateTime.TryParse(value.ToString(), out dateValue))
        {
            return new ValidationResult("Incorrect date format");
        }
        var dayOfWeek = dateValue.DayOfWeek;
        if (dayOfWeek == DayOfWeek.Sunday || dayOfWeek == DayOfWeek.Saturday)
        {
            return new ValidationResult("Joined date is Saturday or Sunday. Please select a different date.");
        }

        return ValidationResult.Success;
    }
}

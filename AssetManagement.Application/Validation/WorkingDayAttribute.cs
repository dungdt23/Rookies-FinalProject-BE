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
        var dayOfWeek = (int)dateValue.DayOfWeek;
        if (dayOfWeek == 0 || dayOfWeek == 6)
        {
            return new ValidationResult("Join date cannot be in Saturnday or Sunday");
        }

        return ValidationResult.Success;
    }
}

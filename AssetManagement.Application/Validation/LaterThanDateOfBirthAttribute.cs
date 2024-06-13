using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AssetManagement.Application.Validation;

public class LaterThanDateOfBirthAttribute : ValidationAttribute
{
    private readonly string _dateOfBirthPropertyName;
    public LaterThanDateOfBirthAttribute(string dateOfBirth)
    {
        _dateOfBirthPropertyName = dateOfBirth;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var dateOfBirthProperty = validationContext.ObjectType.GetProperty(_dateOfBirthPropertyName);

        if (dateOfBirthProperty == null)
        {
            return new ValidationResult($"Can't find property : {_dateOfBirthPropertyName}");
        }

        var dateOfBirthValue = dateOfBirthProperty.GetValue(validationContext.ObjectInstance, null);

        if ((DateTime)value <= (DateTime)dateOfBirthValue)
        {
            return new ValidationResult("Joined date is not later than Date of Birth. Please select a different date.");
        }

        return ValidationResult.Success;
    }
}

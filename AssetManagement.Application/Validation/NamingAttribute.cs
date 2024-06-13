using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AssetManagement.Application.Validation;

public class NamingAttribute : ValidationAttribute
{
    private const int MIN_LENGTH = 2;
    private const int MAX_LENGTH = 100;
    private static readonly Regex ALPHABETIC_WHITESPACE_REGEX = new Regex(@"^[a-zA-Z\s]*$", RegexOptions.Compiled);

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        string stringValue = value.ToString();

        if (stringValue.Length < MIN_LENGTH || stringValue.Length > MAX_LENGTH)
        {
            return new ValidationResult($"The name must be {MIN_LENGTH}-{MAX_LENGTH} characters long.");
        }

        if (!ALPHABETIC_WHITESPACE_REGEX.IsMatch(stringValue))
        {
            return new ValidationResult("The name should contrain alphabetic characters");
        }

        return ValidationResult.Success;
    }

}

using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Identity.API.Validator.Attribute;

public class TimeSpanRangeAttribute(string min, string max) : ValidationAttribute
{
    private readonly TimeSpan min = TimeSpan.Parse(min);
    private readonly TimeSpan max = TimeSpan.Parse(max);

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is TimeSpan timeSpanValue)
        {
            if (timeSpanValue < min || timeSpanValue > max)
            {
                return new ValidationResult(ErrorMessage ?? $"The field {validationContext.DisplayName} must be between {min} and {max}.");
            }
        }

        return ValidationResult.Success!;
    }
}
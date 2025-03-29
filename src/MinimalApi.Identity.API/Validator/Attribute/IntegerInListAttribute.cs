using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Identity.API.Validator.Attribute;

public class IntegerInListAttribute(params int[] validValues) : ValidationAttribute
{
    private readonly List<int> validValues = validValues.ToList();

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is int intValue && validValues.Contains(intValue))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(ErrorMessage ?? $"The value must be one of the following: {string.Join(", ", validValues)}");
    }
}
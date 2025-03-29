using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Identity.API.Validator.Attribute;

public class GreaterThanAttribute(string comparisonProperty) : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is int currentValue)
        {
            var property = validationContext.ObjectType.GetProperty(comparisonProperty);

            if (property == null)
            {
                return new ValidationResult($"Unknown property: {comparisonProperty}");
            }

            var comparisonValue = property.GetValue(validationContext.ObjectInstance);

            if (comparisonValue == null)
            {
                return new ValidationResult($"The comparison property {comparisonProperty} cannot be null.");
            }

            if (currentValue <= (int)comparisonValue)
            {
                return new ValidationResult(ErrorMessage ?? $"The field {validationContext.DisplayName} must be greater than {comparisonProperty}.");
            }
        }

        return ValidationResult.Success!;
    }
}
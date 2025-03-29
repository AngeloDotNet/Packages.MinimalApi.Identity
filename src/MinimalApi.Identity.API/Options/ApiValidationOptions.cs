using System.ComponentModel.DataAnnotations;
using MinimalApi.Identity.API.Validator.Attribute;

namespace MinimalApi.Identity.API.Options;

public class ApiValidationOptions
{
    [Required, Range(1, int.MaxValue, ErrorMessage = "MinLengthFirstName must be greater than zero.")]
    public int MinLengthFirstName { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MaxLengthFirstName must be greater than zero.")]
    [GreaterThan("MinLengthFirstName", ErrorMessage = "MaxLengthFirstName must be greater than MinLengthFirstName.")]
    public int MaxLengthFirstName { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MinLengthLastName must be greater than zero.")]
    public int MinLengthLastName { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MaxLengthLastName must be greater than zero.")]
    [GreaterThan("MinLengthLastName", ErrorMessage = "MaxLengthLastName must be greater than MinLengthLastName.")]
    public int MaxLengthLastName { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MinLengthUsername must be greater than zero.")]
    public int MinLengthUsername { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MaxLengthUsername must be greater than zero.")]
    [GreaterThan("MinLengthUsername", ErrorMessage = "MaxLengthUsername must be greater than MinLengthUsername.")]
    public int MaxLengthUsername { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MinLengthRoleName must be greater than zero.")]
    public int MinLengthRoleName { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MaxLengthRoleName must be greater than zero.")]
    [GreaterThan("MinLengthRoleName", ErrorMessage = "MaxLengthRoleName must be greater than MinLengthRoleName.")]
    public int MaxLengthRoleName { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MinLengthModuleName must be greater than zero.")]
    public int MinLengthModuleName { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MaxLengthModuleName must be greater than zero.")]
    [GreaterThan("MinLengthModuleName", ErrorMessage = "MaxLengthModuleName must be greater than MinLengthModuleName.")]
    public int MaxLengthModuleName { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MinLengthModuleDescription must be greater than zero.")]
    public int MinLengthModuleDescription { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MaxLengthModuleDescription must be greater than zero.")]
    [GreaterThan("MinLengthModuleDescription", ErrorMessage = "MaxLengthModuleDescription must be greater than MinLengthModuleDescription.")]
    public int MaxLengthModuleDescription { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MinLengthLicenseName must be greater than zero.")]
    public int MinLengthLicenseName { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MaxLengthLicenseName must be greater than zero.")]
    [GreaterThan("MinLengthLicenseName", ErrorMessage = "MaxLengthLicenseName must be greater than MinLengthLicenseName.")]
    public int MaxLengthLicenseName { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MinLengthClaimValue must be greater than zero.")]
    public int MinLengthClaimValue { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MaxLengthClaimValue must be greater than zero.")]
    [GreaterThan("MinLengthClaimValue", ErrorMessage = "MaxLengthClaimValue must be greater than MinLengthClaimValue.")]
    public int MaxLengthClaimValue { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MinLengthPolicyName must be greater than zero.")]
    public int MinLengthPolicyName { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MaxLengthPolicyName must be greater than zero.")]
    [GreaterThan("MinLengthPolicyName", ErrorMessage = "MaxLengthPolicyName must be greater than MinLengthPolicyName.")]
    public int MaxLengthPolicyName { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MinLengthPolicyDescription must be greater than zero.")]
    public int MinLengthPolicyDescription { get; init; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "MaxLengthPolicyDescription must be greater than zero.")]
    [GreaterThan("MinLengthPolicyDescription", ErrorMessage = "MaxLengthPolicyDescription must be greater than MinLengthPolicyDescription.")]
    public int MaxLengthPolicyDescription { get; init; }
}

﻿namespace MinimalApi.Identity.API.Options;

public class ApiValidationOptions
{
    public int MinLengthFirstName { get; init; }
    public int MaxLengthFirstName { get; init; }
    public int MinLengthLastName { get; init; }
    public int MaxLengthLastName { get; init; }
    public int MinLengthUsername { get; init; }
    public int MaxLengthUsername { get; init; }
    public int MinLengthRoleName { get; init; }
    public int MaxLengthRoleName { get; init; }
    public int MinLengthModuleName { get; init; }
    public int MaxLengthModuleName { get; init; }
    public int MinLengthModuleDescription { get; init; }
    public int MaxLengthModuleDescription { get; init; }
    public int MinLengthLicenseName { get; init; }
    public int MaxLengthLicenseName { get; init; }
    public int MinLengthClaimValue { get; init; }
    public int MaxLengthClaimValue { get; init; }
    public int MinLengthPolicyName { get; init; }
    public int MaxLengthPolicyName { get; init; }
    public int MinLengthPolicyDescription { get; init; }
    public int MaxLengthPolicyDescription { get; init; }
}
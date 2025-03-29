using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Identity.API.Options;

public class HostedServiceOptions
{
    [Required, Range(1, int.MaxValue, ErrorMessage = "IntervalAuthPolicyUpdaterMinutes must be greater than zero.")]
    public int IntervalAuthPolicyUpdaterMinutes { get; set; }
}
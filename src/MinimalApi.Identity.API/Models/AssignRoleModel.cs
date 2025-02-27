namespace MinimalApi.Identity.API.Models;

public class AssignRoleModel
{
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
}
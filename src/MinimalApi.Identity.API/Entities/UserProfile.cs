using MinimalApi.Identity.API.Entities.Common;

namespace MinimalApi.Identity.API.Entities;

public partial class UserProfile : BaseEntity
{
    public UserProfile(int userId, string firstName, string lastName)
    {
        ChangeUserId(userId);
        ChangeFirstName(firstName);
        ChangeLastName(lastName);
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    //public bool IsEnabled { get; set; }
    //public DateOnly? LastChangePassword { get; set; }
    public int UserId { get; private set; }
    public ApplicationUser User { get; private set; }

    public void ChangeUserId(int userId)
    {
        UserId = userId switch
        {
            <= 0 => throw new ArgumentOutOfRangeException(nameof(userId), "UserId must be greater than zero."),
            _ => userId,
        };
    }

    public void ChangeFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentNullException(nameof(firstName), "FirstName cannot be null or empty.");
        }
        else
        {
            FirstName = firstName;
        }
    }

    public void ChangeLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentNullException(nameof(lastName), "LastName cannot be null or empty.");
        }

        LastName = lastName;
    }
}
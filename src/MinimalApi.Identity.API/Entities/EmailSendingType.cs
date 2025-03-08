using MinimalApi.Identity.API.Entities.Common;

namespace MinimalApi.Identity.API.Entities;

public class EmailSendingType : BaseEntity
{
    public string EmailType { get; set; } = null!;
    public ICollection<EmailSending> EmailSendings { get; set; } = [];
}
using MinimalApi.Identity.API.Entities.Common;

namespace MinimalApi.Identity.API.Entities;

public class EmailSending : BaseEntity
{
    public int EmailSendingTypeId { get; set; }
    public EmailSendingType EmailSendingType { get; set; } = null!;
    public string EmailTo { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
    public bool Sent { get; set; }
    public DateTime DateSent { get; set; }
    public string? ErrorMessage { get; set; } //Consente valore NULL
    public string? ErrorDetails { get; set; } //Consente valore NULL
}
using MinimalApi.Identity.API.Entities.Common;
using MinimalApi.Identity.API.Enums;

namespace MinimalApi.Identity.API.Entities;

public class EmailSending : BaseEntity
{
    public EmailSendingType EmailSendingType { get; set; }
    public string EmailTo { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
    public bool Sent { get; set; }
    public DateTime DateSent { get; set; }
    public string? ErrorMessage { get; set; } //Consente valore NULL
    public string? ErrorDetails { get; set; } //Consente valore NULL
}
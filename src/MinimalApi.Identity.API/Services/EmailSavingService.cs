using MinimalApi.Identity.API.Database;
using MinimalApi.Identity.API.Entities;
using MinimalApi.Identity.API.Services.Interfaces;

namespace MinimalApi.Identity.API.Services;

public class EmailSavingService(MinimalApiDbContext dbContext) : IEmailSavingService
{
    public async Task SaveEmailAsync(EmailSending email)
    {
        dbContext.EmailSendings.Add(email);
        await dbContext.SaveChangesAsync();
    }
}
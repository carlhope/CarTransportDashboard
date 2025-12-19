using Shared.Models;

namespace CarTransportDashboard.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(Email email);
    }
}
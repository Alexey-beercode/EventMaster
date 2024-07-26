using EventMaster.Domain.Models;

namespace EventMaster.BLL.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EventUpdateEmail eventUpdateEmail,CancellationToken cancellationToken=default);
}
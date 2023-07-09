using FAPIServer.Validation.Models;

namespace FAPIServer.Services;

public interface ICibaUserNotificationService
{
    Task SendNotificationAsync(ValidatedCibaRequest validatedRequest, CancellationToken cancellationToken = default);
}

namespace TicketSystem.Application.Common.Interfaces;

public interface IUserPermissionProvider
{
    Task<IReadOnlyCollection<string>> GetPermissionCodesAsync(
        long userId,
        CancellationToken cancellationToken = default);
}

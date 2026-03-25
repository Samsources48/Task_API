using Domain.Entities.seguridad;

namespace Application.Features.Seguridad.Interfaces;

public interface IUserSyncService
{
    Task<User> SyncUserAsync(string clerkId, string email, string userName, CancellationToken ct = default);
    Task UpdateUserAsync(string clerkId, string email, string userName, CancellationToken ct = default);
    Task DeleteUserAsync(string clerkId, CancellationToken ct = default);
}

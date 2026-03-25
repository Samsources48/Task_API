using Application.Features.Seguridad.Interfaces;
using Domain.Entities.seguridad;
using Domain.Interfaces.Seguridad;
using Microsoft.Extensions.Logging;

namespace Application.Features.Seguridad.Operations;

public class UserSyncService(IUserRepository userRepository, ILogger<UserSyncService> logger) : IUserSyncService
{
    public async Task<User> SyncUserAsync(string clerkId, string email, string userName, CancellationToken ct = default)
    {
        var user = await userRepository.GetByClerkIdAsync(clerkId);

        if (user == null)
        {
            logger.LogInformation("Creating new shadow user for Clerk ID: {ClerkId}", clerkId);
            
            user = new User
            {
                ClerkId = clerkId,
                Email = email,
                UserName = userName,
                GuidUser = Guid.NewGuid().ToString(),
                Activo = true,
                FechaRegistro = DateTime.UtcNow,
                UsuarioRegistro = "Clerk_Sync"
            };

            await userRepository.CreateAsync(user);
        }
        else if (user.Email != email || user.UserName != userName)
        {
            // Update profile if changed
            user.Email = email;
            user.UserName = userName;
            user.FechaModificacion = DateTime.UtcNow;
            user.UsuarioModificacion = "Clerk_Sync_Update";
            
            await userRepository.UpdateAsync(user);
        }

        return user;
    }

    public async Task UpdateUserAsync(string clerkId, string email, string userName, CancellationToken ct = default)
    {
        var user = await userRepository.GetByClerkIdAsync(clerkId);
        if (user != null)
        {
            user.Email = email;
            user.UserName = userName;
            user.FechaModificacion = DateTime.UtcNow;
            user.UsuarioModificacion = "Clerk_Webhook_Update";
            await userRepository.UpdateAsync(user);
        }
    }

    public async Task DeleteUserAsync(string clerkId, CancellationToken ct = default)
    {
        var user = await userRepository.GetByClerkIdAsync(clerkId);
        if (user != null)
        {
            user.Activo = false;
            user.FechaEliminacion = DateTime.UtcNow;
            user.UsuarioEliminacion = "Clerk_Webhook_Delete";
            await userRepository.UpdateAsync(user);
        }
    }
}

using Application.Features.Seguridad.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Api.Middlewares;

public class SyncUserMiddleware(RequestDelegate next)
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public async Task InvokeAsync(HttpContext context, IUserSyncService userSyncService, IMemoryCache cache)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var clerkId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                          context.User.FindFirst("sub")?.Value;

            if (!string.IsNullOrEmpty(clerkId))
            {
                var cacheKey = $"clerk_id_mapping_{clerkId}";

                if (!cache.TryGetValue(cacheKey, out long userId))
                {
                    var email = context.User.FindFirst(ClaimTypes.Email)?.Value ?? 
                               context.User.FindFirst("email")?.Value ?? string.Empty;
                    var name = context.User.FindFirst(ClaimTypes.Name)?.Value ?? 
                              context.User.FindFirst("name")?.Value ?? "Clerk User";

                    var user = await userSyncService.SyncUserAsync(clerkId, email, name);
                    userId = user.IdUser;

                    cache.Set(cacheKey, userId, CacheDuration);
                }

                context.Items["UserId"] = userId;
            }
        }

        await next(context);
    }
}

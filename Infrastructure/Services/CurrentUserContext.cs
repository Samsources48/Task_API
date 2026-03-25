using Application.Features.Seguridad.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services;

public class CurrentUserContext(IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    public long GetCurrentUserId()
    {
        if (httpContextAccessor.HttpContext?.Items["UserId"] is long userId)
        {
            return userId;
        }

        return 0;
    }

    public string GetCurrentClerkId()
    {
        var claim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        return claim?.Value ?? string.Empty;
    }
}

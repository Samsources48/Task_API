namespace Application.Features.Seguridad.Interfaces;

public interface ICurrentUserContext
{
    long GetCurrentUserId();
    string GetCurrentClerkId();
}

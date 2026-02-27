using Domain.Entities.seguridad;

namespace Application.Features.Seguridad.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user);
    }
}

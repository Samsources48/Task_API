using Application.DTOs.Seguridad;

namespace Application.Features.Seguridad.Interfaces
{
    public interface IAuthOperation
    {
        Task<AuthResponseDto> Login(LoginDto dto);
    }
}

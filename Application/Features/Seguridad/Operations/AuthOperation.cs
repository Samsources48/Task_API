using Application.DTOs.Seguridad;
using Application.Exceptions;
using Application.Features.Mappings;
using Application.Features.Seguridad.Interfaces;
using Domain.Interfaces.Seguridad;
using Entity = Domain.Entities.seguridad;

namespace Application.Features.Seguridad.Operations
{
    public class AuthOperation(IUserRepository userRepository, IJwtService jwtService) : IAuthOperation
    {
        public async Task<AuthResponseDto> Login(LoginDto dto)
        {
            var user = await userRepository.GetByUsernameAsync(dto.UserName);
            if (user == null || !user.Activo)
                throw new BadRequestException("Credenciales inválidas");

            bool validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!validPassword)
                throw new BadRequestException("Credenciales inválidas");

            var token = jwtService.GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(24),
                User = SeguridadMapper.ToDto(user)
            };
        }
    }
}

using Application.DTOs.Seguridad;

namespace Application.Features.Seguridad.Interfaces
{
    public interface IUsuariosOperation
    {
        Task<List<UserDto>> GetAll();
        Task<UserDto> GetById(long id);
        Task<UserDto> Create(CreateUserDto dto);
        Task<UserDto> Update(UpdateUserDto dto);
        Task<bool> Delete(long id);
    }
}

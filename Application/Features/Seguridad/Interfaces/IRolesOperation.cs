using Application.DTOs.Seguridad;

namespace Application.Features.Seguridad.Interfaces
{
    public interface IRolesOperation
    {
        Task<List<RoleDto>> GetAll();
        Task<RoleDto> Create(CreateRoleDto dto);
        Task<bool> Delete(long id);
    }
}

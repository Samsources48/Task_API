using Application.DTOs.Seguridad;
using Application.Exceptions;
using Application.Features.Mappings;
using Application.Features.Seguridad.Interfaces;
using Domain.Interfaces.Seguridad;

namespace Application.Features.Seguridad.Operations
{
    public class RolesOperation(IRoleRepository roleRepository) : IRolesOperation
    {
        public async Task<List<RoleDto>> GetAll()
        {
            var roles = await roleRepository.GetAllAsync();
            return roles.Select(SeguridadMapper.ToMap).ToList();
        }   

        public async Task<RoleDto> Create(CreateRoleDto dto)
        {
            var role = SeguridadMapper.ToMap(dto);
            await roleRepository.CreateAsync(role);
            return SeguridadMapper.ToMap(role);
        }

        public async Task<bool> Delete(long id)
        {
            var role = await roleRepository.DeleteAsync(id)
                        ?? throw new NotFoundException("Rol no encontrado");
            return true;
        }
    }
}

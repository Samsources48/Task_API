using Application.DTOs.Seguridad;
using Application.Exceptions;
using Application.Features.Mappings;
using Application.Features.Seguridad.Interfaces;
using Domain.Entities.seguridad;
using Domain.Interfaces.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Seguridad.Operations
{
    public class UsuariosOperation(IUserRepository userRepository, IRoleRepository roleRepository) : IUsuariosOperation
    {
        public async Task<List<UserDto>> GetAll()
        {
            var users = await userRepository.GetAllAsync(x => x.Activo, x => x.Roles);
            return users.Where(u => u.Activo)
                 .Select(SeguridadMapper.ToDto)
                 .ToList();
        }

        public async Task<UserDto> GetById(long id)
        {
            var user = await userRepository.GetByIdAsync(id, u => u.Roles)
                            ?? throw new NotFoundException($"Usuario con ID {id} no encontrado");
            return SeguridadMapper.ToDto(user);
        }

        public async Task<UserDto> Create(CreateUserDto dto)
        {
            var existing = await userRepository.GetByUsernameAsync(dto.UserName);
            if (existing is not null)
                throw new ConflictException("El nombre de usuario ya existe");

            var user = SeguridadMapper.ToEntity(dto);
            // Password management is now delegated to Clerk
            user.GuidUser = Guid.NewGuid().ToString();

            if (dto.IdRoles is not null && dto.IdRoles.Any())
            {
                var roles = await roleRepository.Queryable
                    .Where(r => dto.IdRoles.Contains(r.IdRole))
                    .ToListAsync();

                if (!roles.Any())
                    throw new NotFoundException("Roles no encontrados");

                foreach (var role in roles)
                {
                    // Fix: Use the domain entity `role` directly instead of mapping it to a DTO.
                    await roleRepository.UpdateAsync(role);
                }
            }
            await userRepository.CreateAsync(user);
            return SeguridadMapper.ToDto(user);
        }

        public async Task<UserDto> Update(UpdateUserDto dto)
        {
            var user = await userRepository.GetByIdAsync(dto.IdUser, u => u.Roles)
                        ?? throw new NotFoundException("Usuario no encontrado");

            if (!string.IsNullOrEmpty(dto.UserName) && dto.UserName != user.UserName)
            {
                var existing = await userRepository.GetByUsernameAsync(dto.UserName)
                        ?? throw new ConflictException("Nombre de usuario en uso");
                user.UserName = dto.UserName;
            }

            if (dto.Activo.HasValue)
                user.Activo = dto.Activo.Value;

            if (dto.IdRoles != null)
            {
                user.Roles.Clear();
                var roles = await roleRepository.GetAllAsync(r => dto.IdRoles.Contains(r.IdRole));
                foreach (var role in roles)
                {
                    await roleRepository.CreateAsync(role);
                }
            }

            await userRepository.UpdateAsync(user);
            return SeguridadMapper.ToDto(user);
        }

        public async Task<bool> Delete(long id)
        {
            var user = await userRepository.DeleteAsync(id)
                        ?? throw new NotFoundException("Usuario no encontrado");
            return true;
        }
    }
}

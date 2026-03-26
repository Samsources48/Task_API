using Application.DTOs;
using Application.DTOs.Seguridad;
using Domain.Entities.seguridad;

namespace Application.Features.Mappings
{
    public static class SeguridadMapper
    {
        public static UserDto ToDto(User entity)
        {
            if (entity == null) return new UserDto();
            return new UserDto
            {
                IdUser = entity.IdUser,
                UserName = entity.UserName,
                ClerkId = entity.ClerkId,
                Email = entity.Email,
                GuidUser = entity.GuidUser,
                Activo = entity.Activo,
                Roles = entity.Roles.Select(r => r.RoleName).ToList(),
            };
        }

        public static User ToEntity(CreateUserDto dto)
        {
            return new User
            {
                UserName = dto.UserName,
                Activo = true,
                GuidUser = Guid.NewGuid().ToString(),
                FechaRegistro = DateTime.Now
            };
        }

        public static List<UserDto> ToList(IList<User> value)
        {
            return [.. value.Select(ToDto)];
        }


        public static RoleDto ToMap(Role entity)
        {
            if (entity == null) return new RoleDto();
            return new RoleDto
            {
                IdRole = entity.IdRole,
                RoleName = entity.RoleName,
            };
        }

        public static Role ToMap(CreateRoleDto dto)
        {
            return new Role
            {
                RoleName = dto.RoleName,
                FechaRegistro = DateTime.Now
            };
        }

        public static List<RoleDto> ToMap(IList<Role> value)
        {
            return [.. value.Select(ToMap)];
        }
    }
}

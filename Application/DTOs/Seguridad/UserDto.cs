using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Seguridad
{
    public class UserDto
    {
        public long IdUser { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string ClerkId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? GuidUser { get; set; }
        public bool Activo { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class CreateUserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<long> IdRoles { get; set; } = new();
    }

    public class UpdateUserDto
    {
        public long IdUser { get; set; }
        public string? UserName { get; set; }
        public bool? Activo { get; set; }
        public List<long>? IdRoles { get; set; }
    }
}

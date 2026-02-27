namespace Application.DTOs.Seguridad
{
    public class RoleDto
    {
        public long IdRole { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }

    public class CreateRoleDto
    {
        public string RoleName { get; set; } = string.Empty;
    }
}

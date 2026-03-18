using Domain.Entities.Base;

namespace Application.DTOs
{
    public class TaskCategoryDto : AuditProperties
    {
        public long IdTaskCategory { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    public class SaveTaskCategoryDto
    {
        public long? IdTaskCategory { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool? Activo { get; set; }
    }
}
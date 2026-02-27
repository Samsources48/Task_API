using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Base
{
    public class AuditProperties
    {
        public bool Activo { get; set; }

        [Required]
        [StringLength(100)]
        public string UsuarioRegistro { get; set; } = "SYSTEM";
        [Required]
        public DateTime FechaRegistro { get; set; }
        [StringLength(50)]
        public string? IpRegistro { get; set; }
        [StringLength(100)]
        public string? UsuarioModificacion { get; set; }
        [StringLength(100)]
        public DateTime? FechaModificacion { get; set; }
        [StringLength(50)]
        public string? IpModificacion { get; set; }
        public string? UsuarioEliminacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
        [StringLength(50)]
        public string? IpEliminacion { get; set; }
    }
}

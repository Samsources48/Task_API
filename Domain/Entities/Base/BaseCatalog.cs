using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Base
{
    public class BaseCatalog :BaseEntity
    {
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Descripcion { get; set; }
    }
}

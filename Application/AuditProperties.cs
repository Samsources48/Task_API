namespace Application
{
    public abstract class AuditProperties
    {
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool Activo { get; set; }
    }
}

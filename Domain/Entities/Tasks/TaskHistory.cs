using Domain.Entities.Base;
using Domain.Entities.seguridad;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Tasks
{
    [Table(nameof(TaskHistory), Schema = "Tasks")]
    public class TaskHistory : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdTaskHistory { get; set; }

        [ForeignKey(nameof(TaskItem))]
        public long IdTaskItem { get; set; }
        public virtual TaskItem? TaskItem { get; set; }

        [ForeignKey(nameof(User))]
        public long IdUser { get; set; }
        public virtual User? User { get; set; }

        [Required]
        [StringLength(50)]
        public string PreviousStatus { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string NewStatus { get; set; } = string.Empty;

        public DateTime ChangedAt { get; set; }

        /// <summary>
        /// Minutos que la tarea estuvo en PreviousStatus antes de este cambio.
        /// </summary>
        public int? DurationMinutes { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }
    }
}

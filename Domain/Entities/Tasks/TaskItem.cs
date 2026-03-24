using Domain.Entities.Base;
using Domain.Entities.seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Tasks
{
    [Table(nameof(TaskItem), Schema = ("Tasks"))]
    public class TaskItem : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdTaskItem { get; set; }
        [MaxLength(250)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsNotified { get; set; } = false;

        [ForeignKey(nameof(TaskCategory))]
        public long? IdTaskCategory { get; set; }
        public virtual TaskCategory? TaskCategory { get; set; }

        [ForeignKey(nameof(User))]
        public long IdUser { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public virtual User? User { get; set; }
    }
}


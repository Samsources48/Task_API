using Domain.Entities.Base;
using Domain.Entities.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.seguridad
{
    [Table(nameof(User), Schema ="SEG")]
    public class User : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public long IdUser { get; set; }

        [Required]
        [StringLength(200)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(100)]
        public string? GuidUser { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

        public virtual ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    }
}

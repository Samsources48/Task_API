using Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Tasks
{
    [Table(nameof(TaskCategory), Schema = ("Tasks"))]
    public class TaskCategory:  BaseCatalog
    {
        public long IdTaskCategory { get; set; }
        public virtual ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    }
}

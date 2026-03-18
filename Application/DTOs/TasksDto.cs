using Domain.Entities.seguridad;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class TasksDto : AuditProperties
    {
        public long IdTaskItem { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public priorityEnum Priority { get; set; }
        public statusTasksEnum Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? IdTaskCategory { get; set; }
        public bool IsCompleted { get; set; } = false;
        public long IdUser { get; set; }
        public string UserName { get; set; } = string.Empty;
    }

    public class SaveTasksDto
    {
        public long? IdTaskItem { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public priorityEnum Priority { get; set; }
        public statusTasksEnum Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? IdTaskCategory { get; set; }
        public bool? IsCompleted { get; set; }
        public long IdUser { get; set; }
    }

    public class TaskDashboard
    {
        public long? TotalTasks { get; set; }
        public long? CompletedTasks { get; set; }
        public long? InProgressTasks { get; set; }

    }
}

using Domain.Entities.seguridad;
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
        public int Priority { get; set; }
        public int Status { get; set; }
        public DateTime? DueData { get; set; }
        public bool IsCompleted { get; set; } = false;
        public long IdUser { get; set; }
        public string UserName { get; set; } = string.Empty;
    }

    public class SaveTasksDto
    {
        public long? IdTaskItem { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; }
        public DateTime? DueData { get; set; }
        public bool? IsCompleted { get; set; }
        public long IdUser { get; set; }
    }
}

using Domain.Enums;

namespace Application.DTOs
{
    public class TaskHistoryDto
    {
        public long IdTaskHistory { get; set; }
        public long IdTaskItem { get; set; }
        public long IdUser { get; set; }
        public taskHistoryStatusEnum PreviousStatus { get; set; }
        public taskHistoryStatusEnum NewStatus { get; set; }
        public DateTime ChangedAt { get; set; }
        public int? DurationMinutes { get; set; }
        public string? Remarks { get; set; }
        public bool Activo { get; set; }
    }

    public class SaveTaskHistoryDto
    {
        public long IdTaskItem { get; set; }
        public long IdUser { get; set; }
        public taskHistoryStatusEnum PreviousStatus { get; set; }
        public taskHistoryStatusEnum NewStatus { get; set; }
        public DateTime? ChangedAt { get; set; }
        public int? DurationMinutes { get; set; }
        public string? Remarks { get; set; }
    }

    public class UpdateTaskHistoryDto
    {
        public long IdTaskHistory { get; set; }
        public long IdTaskItem { get; set; }
        public long IdUser { get; set; }
        public taskHistoryStatusEnum PreviousStatus { get; set; }
        public taskHistoryStatusEnum NewStatus { get; set; }
        public DateTime? ChangedAt { get; set; }
        public int? DurationMinutes { get; set; }
        public string? Remarks { get; set; }
    }
}

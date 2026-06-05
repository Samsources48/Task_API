using Application.DTOs;
using Domain.Entities.Tasks;
using Domain.utils;

namespace Application.Features.Mappings
{
    public static class TaskHistoryMapper
    {
        public static TaskHistoryDto toDto(TaskHistory entity)
        {
            if (entity == null) return new TaskHistoryDto();

            return new TaskHistoryDto
            {
                IdTaskHistory = entity.IdTaskHistory,
                IdTaskItem = entity.IdTaskItem,
                IdUser = entity.IdUser,
                PreviousStatus = entity.PreviousStatus.toTaskHistoryStatusEnum(),
                NewStatus = entity.NewStatus.toTaskHistoryStatusEnum(),
                ChangedAt = entity.ChangedAt,
                DurationMinutes = entity.DurationMinutes,
                Remarks = entity.Remarks,
                Activo = entity.Activo
            };
        }

        public static TaskHistory toEntity(SaveTaskHistoryDto dto)
        {
            if (dto == null) return new TaskHistory();

            return new TaskHistory
            {
                IdTaskItem = dto.IdTaskItem,
                IdUser = dto.IdUser,
                PreviousStatus = dto.PreviousStatus.toStringStatus(),
                NewStatus = dto.NewStatus.toStringStatus(),
                ChangedAt = dto.ChangedAt ?? DateTime.UtcNow,
                DurationMinutes = dto.DurationMinutes,
                Remarks = dto.Remarks,
                Activo = true
            };
        }

        public static TaskHistory toEntity(UpdateTaskHistoryDto dto)
        {
            if (dto == null) return new TaskHistory();

            return new TaskHistory
            {
                IdTaskHistory = dto.IdTaskHistory,
                IdTaskItem = dto.IdTaskItem,
                IdUser = dto.IdUser,
                PreviousStatus = dto.PreviousStatus.toStringStatus(),
                NewStatus = dto.NewStatus.toStringStatus(),
                ChangedAt = dto.ChangedAt ?? DateTime.UtcNow,
                DurationMinutes = dto.DurationMinutes,
                Remarks = dto.Remarks,
                Activo = true
            };
        }

        public static List<TaskHistoryDto> Map(List<TaskHistory> items)
        {
            return [..items.Select(e => toDto(e))];
        }
    }
}

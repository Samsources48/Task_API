using Application.DTOs;
using Domain.Entities.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Mappings
{
    public static class TasksMapper
    {
        public static TasksDto toDto(TaskItem entity)
        {
            if (entity == null) return new TasksDto();

            return new TasksDto
            {
                IdTaskItem = entity.IdTaskItem,
                Title = entity.Title,
                Description = entity.Description,
                Priority = entity.Priority,
                Status = entity.Status,
                DueData = entity.DueData,
                IsCompleted = entity.IsCompleted,
                IdUser = entity.User?.IdUser ?? 0,
                UserName = entity.User?.UserName ?? string.Empty
            };
        }


        public static TaskItem toEntity(SaveTasksDto dto)
        {
            if (dto == null) return new TaskItem();
            return new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                IsCompleted = false,
                Priority = dto.Priority,
                Status = dto.Status,
                DueData = dto.DueData,
                IdUser = dto.IdUser
            };
        }

        public static List<TasksDto> Map(List<TaskItem> tasks)
        {
            return [..tasks.Select(t => toDto(t))];
        }
    }
}

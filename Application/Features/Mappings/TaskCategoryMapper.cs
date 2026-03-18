using Application.DTOs;
using Domain.Entities.Tasks;

namespace Application.Features.Mappings
{
    public static class TaskCategoryMapper
    {
        public static TaskCategoryDto toDto(TaskCategory entity)
        {
            if (entity == null) return new TaskCategoryDto();

            return new TaskCategoryDto
            {
                IdTaskCategory = entity.IdTaskCategory,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo
            };
        }

        public static TaskCategory toEntity(SaveTaskCategoryDto dto)
        {
            if (dto == null) return new TaskCategory();
            return new TaskCategory
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Activo = dto.Activo ?? true
            };
        }

        public static List<TaskCategoryDto> Map(List<TaskCategory> categories)
        {
            return [..categories.Select(c => toDto(c))];
        }
    }
}
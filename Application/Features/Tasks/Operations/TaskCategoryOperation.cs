using Application.DTOs;
using Application.Exceptions;
using Application.Features.Mappings;
using Application.Features.Tasks.Interfaces;
using Domain.Interfaces.Tasks;

namespace Application.Features.Tasks.Operations
{
    public class TaskCategoryOperation(ITaskCategoryRepository taskCategoryRepository) : ITaskCategoryOperation
    {
        public async Task<List<TaskCategoryDto>> GetAll()
        {
            var response = await taskCategoryRepository.GetAllAsync(x => x.Activo);
            return TaskCategoryMapper.Map(response.ToList());
        }

        public async Task<TaskCategoryDto> GetById(int id)
        {
            var response = await taskCategoryRepository.GetByIdAsync(id);
            if (response == null)
                throw new NotFoundException($"TaskCategory con ID {id} no encontrado");
            return TaskCategoryMapper.toDto(response);
        }

        public async Task<TaskCategoryDto> Create(SaveTaskCategoryDto dto)
        {
            if (dto == null)
                throw new BadRequestException("El objeto TaskCategory no puede ser nulo");

            var category = TaskCategoryMapper.toEntity(dto);

            var created = await taskCategoryRepository.CreateAsync(category);

            if (created == null)
                throw new BadRequestException("No se pudo guardar la categoría de tarea");

            return TaskCategoryMapper.toDto(created);
        }

        public async Task<TaskCategoryDto> Update(SaveTaskCategoryDto dto)
        {
            if (!dto.IdTaskCategory.HasValue)
                throw new BadRequestException("El ID de la categoría es requerido para actualizar");

            var existing = await taskCategoryRepository.GetByIdAsync(dto.IdTaskCategory.Value)
                            ?? throw new NotFoundException($"TaskCategory con ID {dto.IdTaskCategory} no encontrado");

            var updatedEntity = TaskCategoryMapper.toEntity(dto);
            updatedEntity.IdTaskCategory = dto.IdTaskCategory.Value;

            var updated = await taskCategoryRepository.UpdateAsync(existing.IdTaskCategory, updatedEntity);

            if (updated == null)
                throw new BadRequestException("No se pudo actualizar la categoría de tarea");

            return TaskCategoryMapper.toDto(updated);
        }

        public async Task<bool> Delete(int id)
        {
            var existing = await taskCategoryRepository.GetByIdAsync(id)
                            ?? throw new NotFoundException($"TaskCategory con ID {id} no encontrado");

            await taskCategoryRepository.DeleteAsync(id);

            return true;
        }
    }
}
using Application.DTOs;
using Application.Exceptions;
using Application.Features.Mappings;
using Application.Features.Products.Interfaces;
using Domain.Interfaces.Tasks;

namespace Application.Features.Products.Operations
{
    public class TasksOperation(ITaskItemRepository taskRepository) : ITasksOperation
    {

        public async Task<List<TasksDto>> GetAll()
        {
            var response = await taskRepository.GetAllAsync();
            return TasksMapper.Map(response);
        }

        public async Task<TasksDto> GetById(int id)
        {
            var response = await taskRepository.GetByIdAsync(id);
            if (response == null)
                throw new NotFoundException($"Task con ID {id} no encontrado");
            return TasksMapper.toDto(response);
        }

        public async Task<TasksDto> Create(SaveTasksDto dto)
        {
            if (dto == null)
                throw new BadRequestException("El objeto Task no puede ser nulo");

            var producto = TasksMapper.toEntity(dto);

            var created = await taskRepository.CreateAsync(producto);

            if (created == null)
                throw new BadRequestException("No se pudo guardar el producto");

            return TasksMapper.toDto(created);
        }

        public async Task<TasksDto> Update(SaveTasksDto dto)
        {
            var existing = await taskRepository.GetByIdAsync(dto.IdTaskItem.Value)
                            ?? throw new NotFoundException("Tarea no Encontrada");

            if (existing == null)
                throw new NotFoundException($"Task con ID {dto.IdTaskItem} no encontrado");

            var updatedEntity = TasksMapper.toEntity(dto);
            var updated = await taskRepository.UpdateAsync(updatedEntity);

            if (updated == null)
                throw new BadRequestException("No se pudo actualizar el producto");
            return TasksMapper.toDto(updated);
        }

        public async Task<bool> Delete(int id)
        {
            var existing = await taskRepository.GetByIdAsync(id)
                            ?? throw new NotFoundException("Tarea no Encontrada");

            if (existing == null)
                throw new NotFoundException($"Task con ID {id} no encontrado");

            await taskRepository.DeleteAsync(id);

            return true;
        }
    }
}

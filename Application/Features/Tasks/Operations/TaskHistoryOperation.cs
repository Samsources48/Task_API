using Application.DTOs;
using Application.Exceptions;
using Application.Features.Mappings;
using Application.Features.Tasks.Interfaces;
using Domain.Interfaces.Tasks;

namespace Application.Features.Tasks.Operations
{
    public class TaskHistoryOperation(ITaskHistoryRepository taskHistoryRepository) : ITaskHistoryOperation
    {
        public async Task<List<TaskHistoryDto>> GetByTaskItem(long idTaskItem)
        {
            var items = await taskHistoryRepository.GetAllAsync(
                x => x.Activo && x.IdTaskItem == idTaskItem);
            return TaskHistoryMapper.Map(items);
        }

        public async Task<TaskHistoryDto> GetById(long id)
        {
            var entity = await taskHistoryRepository.GetByIdAsync(id);
            if (entity is null)
                throw new NotFoundException($"TaskHistory with ID {id} not found");
            return TaskHistoryMapper.toDto(entity);
        }

        public async Task<TaskHistoryDto> Create(SaveTaskHistoryDto dto)
        {
            var entity = TaskHistoryMapper.toEntity(dto);
            var created = await taskHistoryRepository.CreateAsync(entity);
            return TaskHistoryMapper.toDto(created);
        }

        public async Task<TaskHistoryDto> Update(UpdateTaskHistoryDto dto)
        {
            var existing = await taskHistoryRepository.GetByIdAsync(dto.IdTaskHistory)
                ?? throw new NotFoundException($"TaskHistory with ID {dto.IdTaskHistory} not found");

            var updated = TaskHistoryMapper.toEntity(dto);
            var result = await taskHistoryRepository.UpdateAsync(existing.IdTaskHistory, updated);
            return TaskHistoryMapper.toDto(result);
        }

        public async Task<bool> Delete(long id)
        {
            var existing = await taskHistoryRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"TaskHistory with ID {id} not found");

            await taskHistoryRepository.DeleteAsync(existing.IdTaskHistory);
            return true;
        }
    }
}

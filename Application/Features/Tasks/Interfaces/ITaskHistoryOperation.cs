using Application.DTOs;

namespace Application.Features.Tasks.Interfaces
{
    public interface ITaskHistoryOperation
    {
        Task<List<TaskHistoryDto>> GetByTaskItem(long idTaskItem);
        Task<TaskHistoryDto> GetById(long id);
        Task<TaskHistoryDto> Create(SaveTaskHistoryDto dto);
        Task<TaskHistoryDto> Update(UpdateTaskHistoryDto dto);
        Task<bool> Delete(long id);
    }
}

using Application.DTOs;

namespace Application.Features.Products.Interfaces
{
    public interface ITasksOperation
    {
        Task<TaskDashboard> GetTaskDasboard();
        Task<List<TasksDto>> GetAll();
        Task<TasksDto> GetById(int id);
        Task<TasksDto> Create(SaveTasksDto dto);
        Task<TasksDto> Update(SaveTasksDto dto);
        Task<bool> Delete(int id);
    }
}

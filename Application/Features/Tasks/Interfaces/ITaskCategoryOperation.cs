using Application.DTOs;

namespace Application.Features.Tasks.Interfaces
{
    public interface ITaskCategoryOperation
    {
        Task<List<TaskCategoryDto>> GetAll();
        Task<TaskCategoryDto> GetById(int id);
        Task<TaskCategoryDto> Create(SaveTaskCategoryDto dto);
        Task<TaskCategoryDto> Update(SaveTaskCategoryDto dto);
        Task<bool> Delete(int id);
    }
}
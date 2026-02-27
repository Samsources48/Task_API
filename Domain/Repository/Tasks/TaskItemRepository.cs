using Domain.Entities.Tasks;
using Domain.Interfaces.Tasks;
using Domain.Repository.Base;

namespace Domain.Repository.Tasks
{
    public class TaskItemRepository(SqlDbContext sqlDbContext) : Repository<TaskItem>(sqlDbContext), ITaskItemRepository
    {
    }
}

using Domain.Entities.Tasks;
using Domain.Interfaces.Tasks;
using Domain.Repository.Base;

namespace Domain.Repository.Tasks
{
    public class TaskHistoryRepository(SqlDbContext ctx) : Repository<TaskHistory>(ctx), ITaskHistoryRepository
    {
    }
}

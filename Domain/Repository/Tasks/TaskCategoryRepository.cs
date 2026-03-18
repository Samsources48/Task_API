using Domain.Entities.Tasks;
using Domain.Interfaces.Tasks;
using Domain.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository.Tasks
{
    public class TaskCategoryRepository(SqlDbContext sqlDbContext): Repository<TaskCategory>(sqlDbContext), ITaskCategoryRepository
    {
    }
}

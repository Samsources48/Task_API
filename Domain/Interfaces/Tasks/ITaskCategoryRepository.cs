using Domain.Entities.Tasks;
using Domain.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Tasks
{
    public interface ITaskCategoryRepository: IRepository<TaskCategory>
    {
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.BackGroundJobs.Jobs
{
    public interface INotifyExpiringTasksJob
    {
        Task ExecuteAsync(CancellationToken ct = default);

    }
}

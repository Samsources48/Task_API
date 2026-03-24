using Hangfire;
using Infrastructure.BackGroundJobs.Jobs;
using Infrastructure.Configuration.Hangfire;
using Infrastructure.Services.Notification.Email;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.BackGroundJobs
{
    public static class HangFireJobSetup
    {
        //private const string MorningJobId = "tracking-morning";
        //private const string EveningJobId = "tracking-evening";

        public static void ConfigureHangFireJobs(IServiceProvider serviceProvider)
        {
            var settings = serviceProvider.GetRequiredService<IOptions<HangfireSettings>>().Value;

            //if (!settings.ScheduleJobs.EnableMasiveTracking)
            //{
            //    RecurringJob.RemoveIfExists(MorningJobId);
            //    RecurringJob.RemoveIfExists(EveningJobId);
            //    return;
            //}

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(settings.ScheduleJobs.TimeZone);

            // Registrar Job recurrente de Hangfire: Revisar cada 60 mins
            RecurringJob.AddOrUpdate<INotifyExpiringTasksJob>(
                "notify-expiring-tasks",
                methodCall: job => job.ExecuteAsync(CancellationToken.None),
                Cron.HourInterval(1));
        }
    }
}

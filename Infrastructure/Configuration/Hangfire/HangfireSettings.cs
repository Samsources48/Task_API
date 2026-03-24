using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configuration.Hangfire
{
    public class HangfireSettings
    {

        public const string SectionName = "Hangfire";
        //# de workers que se ejecutarán en paralelo. Por defecto, Hangfire asigna 20 workers.
        public int WorkerCount { get; set; }
        //public int MaxWorkerCount { get; set; }
        public string ServerName { get; set; } = "TrackingServer";
        public bool EnableDashboard { get; set; } = true;
        public string DashboardPath { get; set; } = "/hangfire-dashboard";
        //Configurracion de jobs Progamados
        public ScheduleJobsConfig ScheduleJobs { get; set; } = new();
    }


    public class ScheduleJobsConfig
    {
        public bool EnableMasiveTracking { get; set; } = true; // habilitar para traking masivo programado
        public string MorninngCron { get; set; } = "0 3 * * *"; // Todos los días a las 3:00 AM
        public string EveningCron { get; set; } = "0 18 * * *"; // Todos los días a las 6:00 PM
        public string TimeZone { get; set; } = "America/Guayaquil"; // Zona horaria para la programación de los jobs
    }
}

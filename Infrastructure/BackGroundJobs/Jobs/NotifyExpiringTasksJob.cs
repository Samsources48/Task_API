using Domain;
using Domain.Interfaces.Tasks;
using Infrastructure.Services.Notification.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.BackGroundJobs.Jobs
{
    public class NotifyExpiringTasksJob: INotifyExpiringTasksJob
    {
        private readonly ITaskItemRepository _context;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly ILogger<NotifyExpiringTasksJob> _logger;

        public NotifyExpiringTasksJob(
            ITaskItemRepository context,
            IEmailNotificationService emailNotificationService,
            ILogger<NotifyExpiringTasksJob> logger)
        {
            _context = context;
            _emailNotificationService = emailNotificationService;
            _logger = logger;
        }
        public async Task ExecuteAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Iniciando ejecución del Job de notificación de tareas por vencer...");

            var now = DateTime.Now;
            var windowStart = now;
            var windowEnd = now.AddHours(3);

            var expiringTasks = await _context.Queryable
                .Include(t => t.User)
                .Where(t => !t.IsCompleted &&
                            !t.IsNotified &&
                            t.EndDate.HasValue &&
                            t.EndDate.Value >= windowStart &&
                            t.EndDate.Value <= windowEnd)
                .ToListAsync(ct);

            if (!expiringTasks.Any())
            {
                _logger.LogInformation("No se encontraron tareas próximas a vencer en la ventana de tiempo.");
                return;
            }
            _logger.LogInformation("Se encontraron {Count} tareas por vencer. Enviando notificaciones...", expiringTasks.Count);

            foreach (var task in expiringTasks)
            {
                var email = task.User?.UserName;
                if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                {
                    _logger.LogWarning("No se pudo enviar notificación para la tarea {Id} - {Name}: Usuario {User} no tiene un email válido.", task.IdTaskItem, task.Title, task.User?.UserName);
                    continue;
                }

                await _emailNotificationService.NotifyExpiringTaskAsync(email,task.User?.UserName ?? "Usuario",task.Title,task.EndDate,ct);

                task.IsNotified = true;
            }
            await _context.DbContext.SaveChangesAsync(ct);
            _logger.LogInformation("Job de notificaciones completado exitosamente.");
        }
    }
}

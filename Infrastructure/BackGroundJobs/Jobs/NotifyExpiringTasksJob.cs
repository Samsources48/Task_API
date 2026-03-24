using Domain;
using Infrastructure.Services.Notification.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.BackGroundJobs.Jobs
{
    public class NotifyExpiringTasksJob: INotifyExpiringTasksJob
    {
        private readonly SqlDbContext _context;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly ILogger<NotifyExpiringTasksJob> _logger;

        public NotifyExpiringTasksJob(
            SqlDbContext context,
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
            // Definimos un rango de ventana de 2 horas desde ahora.
            // Para asegurar que cubrimos las tareas con el Job cada 60 mins, buscamos tareas que venzan en las próximas 3 horas
            // y que aún no hayan sido notificadas.
            var windowStart = now;
            var windowEnd = now.AddHours(3);
            var expiringTasks = await _context.TaskItems
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
                // El UserName debe contener el email según la actualización del Seeder
                var email = task.User?.UserName;
                if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                {
                    _logger.LogWarning("No se pudo enviar notificación para la tarea {Id}: Usuario {User} no tiene un email válido.", task.IdTaskItem, task.User?.UserName);
                    continue;
                }
                await _emailNotificationService.NotifyExpiringTaskAsync(
                    recipientEmail: email,
                    userName: task.User?.UserName ?? "Usuario",
                    taskTitle: task.Title,
                    dueDate: task.EndDate,
                    ct: ct);
                // Marcar como notificada para no repetir en la siguiente ejecución del Job
                task.IsNotified = true;
            }
            await _context.SaveChangesAsync(ct);
            _logger.LogInformation("Job de notificaciones completado exitosamente.");
        }
    }
}

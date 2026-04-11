using Application.DTOs;
using Application.Exceptions;
using Application.Features.Mappings;
using Application.Features.Notifications.DTOs;
using Application.Features.Notifications.Interfaces;
using Application.Features.Products.Interfaces;
using Domain;
using Domain.Entities.seguridad;
using Domain.Enums;
using Domain.Interfaces.Tasks;
using Domain.utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.AccessControl;

namespace Application.Features.Products.Operations
{
    public class TasksOperation(ITaskItemRepository taskRepository, IRealTimeNotifier realTimeNotifier) : ITasksOperation
    {
        public async Task<TaskDashboard> GetTaskDasboard(string idUser)
        {
            var response = await taskRepository.GetAllAsync(x => x.Activo && x.IdUser == long.Parse(idUser));
            return new TaskDashboard
            {
                TotalTasks = response.Count(),
                CompletedTasks = response.Count(x => x.IsCompleted),
                InProgressTasks = response.Count(x => x.IsCompleted == false),
            };
        }

        public async Task<List<TasksDto>> GetAll(string idUser)
        {
            var response = await taskRepository.GetAllAsync(x => x.Activo && x.IdUser == long.Parse(idUser), x => x.User!);
            return TasksMapper.Map(response);
        }

        public async Task<TasksDto> GetById(int id)
        {
            var response = await taskRepository.GetByIdAsync(id);
            if (response is null)
                throw new NotFoundException($"Task con ID {id} no encontrado");
            return TasksMapper.toDto(response);
        }

        public async Task<TasksDto> Create(SaveTasksDto dto)
        {
            if (dto is null)
                throw new BadRequestException("El objeto Task no puede ser nulo");

            var isCompleteTask = dto.Status == statusTasksEnum.Done;

            var producto = TasksMapper.toEntity(dto);
            producto.IsCompleted = isCompleteTask;

            var created = await taskRepository.CreateAsync(producto);

            if (created is null)
                throw new BadRequestException("No se pudo guardar el producto");

            return TasksMapper.toDto(created);
        }

        public async Task<TasksDto> Update(SaveTasksDto dto)
        {
            var existing = await taskRepository.GetByIdAsync(dto.IdTaskItem.Value, x => x.User!)
                            ?? throw new NotFoundException("Tarea no Encontrada");

            var isCompleteTask = dto.Status == statusTasksEnum.Done;

            var updatedEntity = TasksMapper.toEntity(dto);
            updatedEntity.IdTaskItem = dto.IdTaskItem.Value;
            updatedEntity.IsCompleted = isCompleteTask;

            var statusChanged = existing.Status != updatedEntity.Status;

            var updated = await taskRepository.UpdateAsync(existing.IdTaskItem, updatedEntity);

            if (updated is null)
                throw new NotFoundException($"Task con ID {dto.IdTaskItem} no encontrado");

            // Notificar cambio de estado en tiempo real
            if (statusChanged && existing.User?.ClerkId != null)
            {
                await realTimeNotifier.NotifyUserAsync(existing.User.ClerkId, new NotificationDto
                {
                    Title = "Estado de tarea actualizado",
                    Message = $"La tarea \"{updated.Title}\" cambió a estado: {updated.Status}",
                    Type = "task_status_changed",
                    TaskId = updated.IdTaskItem
                });
            }

            return TasksMapper.toDto(updated);
        }

        public async Task<bool> Delete(int id)
        {
            var existing = await taskRepository.GetByIdAsync(id)
                            ?? throw new NotFoundException("Tarea no Encontrada");

            if (existing is null)
                throw new NotFoundException($"Task con ID {id} no encontrado");

            await taskRepository.DeleteAsync(id);

            return true;
        }
    }
}

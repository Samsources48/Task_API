using Application.Features.Products.Interfaces;
using Application.Features.Products.Operations;
using Application.Features.Seguridad.Interfaces;
using Application.Features.Seguridad.Operations;
using Application.Features.Tasks.Interfaces;
using Application.Features.Tasks.Operations;
using Domain.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace Application.Extensions
{
    public static class ServiceCollectionExtensionsOperations
    {
        public static IServiceCollection AddServicesLayer(this IServiceCollection services)
        {
            services.AddScoped<IAuthOperation, AuthOperation>();
            services.AddScoped<IUsuariosOperation, UsuariosOperation>();
            services.AddScoped<IRolesOperation, RolesOperation>();
            services.AddScoped<ITasksOperation, TasksOperation>();    
            services.AddScoped<ITaskCategoryOperation, TaskCategoryOperation>();

            services.AddDataAccessServices();
            return services;
        }
    }
}

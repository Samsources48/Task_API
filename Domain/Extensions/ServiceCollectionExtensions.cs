using Domain.Interfaces.Seguridad;
using Domain.Interfaces.Tasks;
using Domain.Repository.Seguridad;
using Domain.Repository.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddDataAccessServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ITaskItemRepository, TaskItemRepository>();

            return services;
        }
    }
}

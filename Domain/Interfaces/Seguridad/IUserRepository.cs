using Domain.Entities.seguridad;
using Domain.Interfaces.Base;


namespace Domain.Interfaces.Seguridad
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
    }
}

using Domain;
using Domain.Entities.seguridad;
using Domain.Interfaces.Seguridad;
using Domain.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository.Seguridad
{
    public class UserRepository(SqlDbContext sqlDbContext) : Repository<User>(sqlDbContext), IUserRepository
    {
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await sqlDbContext.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserName == username);
        }
    }
}

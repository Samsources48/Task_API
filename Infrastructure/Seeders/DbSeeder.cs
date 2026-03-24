using Domain;
using Domain.Entities.seguridad;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeders
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(SqlDbContext context)
        {
            // 1. Seed Roles
            if (!await context.Roles.AnyAsync())
            {
                var adminRole = new Role
                {
                    RoleName = "Admin",
                    FechaRegistro = DateTime.Now,
                    UsuarioRegistro = "System",
                    Activo = true
                };
                
                var userRole = new Role
                {
                    RoleName = "User",
                    FechaRegistro = DateTime.Now,
                    UsuarioRegistro = "System",
                    Activo = true
                };

                await context.Roles.AddRangeAsync(adminRole, userRole);
                await context.SaveChangesAsync();
            }

            // 2. Seed Admin User
            if (!await context.Users.AnyAsync(u => u.UserName == "samuelpachayhf@gmail.com"))
            {
                var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
                if (adminRole != null)
                {
                    var adminUser = new User
                    {
                        UserName = "samuelpachayhf@gmail.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                        GuidUser = Guid.NewGuid().ToString(),
                        Activo = true,
                        FechaRegistro = DateTime.Now,
                        UsuarioRegistro = "System"
                    };

                    adminUser.Roles.Add(adminRole);
                    
                    await context.Users.AddAsync(adminUser);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}

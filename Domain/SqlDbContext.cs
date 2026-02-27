using Domain.Entities.seguridad;
using Domain.Entities.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
    public class SqlDbContext(DbContextOptions<SqlDbContext> options) : DbContext(options)
    {

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<TaskItem> TaskItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRoles",
                    j => j.HasOne<Role>().WithMany().HasForeignKey("IdRole"),
                    j => j.HasOne<User>().WithMany().HasForeignKey("IdUser"),
                    j =>
                     {
                         j.ToTable("UserRoles", "SEG");
                         j.HasKey("IdUser", "IdRole");
                     });

            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.ToTable("TaskItem", "Tasks");
                entity.HasKey(e => e.IdTaskItem);
                entity.HasOne(d => d.User)
                    .WithMany(p => p.TaskItems)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

    }
}

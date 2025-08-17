using AgriFieldHub.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriFieldHub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Field> Fields { get; set; } = null!;
        public DbSet<Controller> Controllers { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Field>()
                .HasOne(f => f.User)
                .WithMany(u => u.Fields)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Controller>()
                .HasOne(c => c.Field)
                .WithMany(f => f.Controllers)
                .HasForeignKey(c => c.FieldId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            // Removed static admin seed; handled at runtime via initializer
        }
    }
}

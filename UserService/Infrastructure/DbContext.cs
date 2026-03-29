using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Infrastructure {
    public class AppDbContext : DbContext {
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasDefaultValueSql("NEWID()")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Email).IsRequired();
                    
                entity.Property(e => e.CreatedAt).IsRequired();
            });
        }

        public override int SaveChanges() {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps() {
            var entries = ChangeTracker
                .Entries()
                .Where(e => (e.Entity is IUpdatedAt) && e.State == EntityState.Modified || (e.Entity is ICreatedAt) && e.State == EntityState.Added);

            foreach (var entityEntry in entries) {
                var entity = entityEntry.Entity;
                var now = DateTimeOffset.UtcNow;

                if (entityEntry.State == EntityState.Added) {
                    ((ICreatedAt)entity).CreatedAt = now;
                }

                ((IUpdatedAt)entity).UpdatedAt = now;
            }
        }
    }
}

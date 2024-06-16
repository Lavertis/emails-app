using EmailsApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailsApp.Database;

public class AppDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Email> Emails { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new())
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    private void UpdateTimestamps()
    {
        var entityEntries = ChangeTracker.Entries()
            .Where(x => x is { Entity: BaseEntity, State: EntityState.Added or EntityState.Modified });

        foreach (var entityEntry in entityEntries)
        {
            var now = DateTime.UtcNow;
            ((BaseEntity)entityEntry.Entity).UpdatedAt = now;
            if (entityEntry.State == EntityState.Added)
                ((BaseEntity)entityEntry.Entity).CreatedAt = now;
        }
    }
}
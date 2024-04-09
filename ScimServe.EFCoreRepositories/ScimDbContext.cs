using Microsoft.EntityFrameworkCore;
using ScimServe.Repositories.Entities;

namespace ScimServe.EFCoreRepositories;

public class ScimDbContext : DbContext
{
    public ScimDbContext(DbContextOptions<ScimDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; init; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("scim")
            .Entity<UserEntity>()
            .HasAlternateKey(u => new { u.Id, u.Version });
    }
}
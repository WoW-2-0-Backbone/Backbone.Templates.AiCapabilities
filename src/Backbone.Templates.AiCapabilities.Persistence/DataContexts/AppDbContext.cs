using Backbone.Templates.AiCapabilities.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Templates.AiCapabilities.Persistence.DataContexts;

public class AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : DbContext(dbContextOptions)
{
    #region Identity Infrastructure

    public DbSet<User> Users => Set<User>();

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
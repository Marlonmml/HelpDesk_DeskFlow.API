using DeskFlow.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeskFlow.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Chamados> Chamados { get; set; }
    public DbSet<Categorias> Categorias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categorias>()
            .HasMany(c => c.Chamados)
            .WithOne(ch => ch.Categoria)
            .HasForeignKey(ch => ch.CategoriaId);
    }
}
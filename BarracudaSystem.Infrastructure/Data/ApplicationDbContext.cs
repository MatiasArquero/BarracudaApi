using BarracudaSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarracudaSystem.Infrastructure.Data;
// FLUENT API----------------------------------------------
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Entidad Item
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id);
            //Datos Basicos [INDEX para consultas rapidas]
            entity.HasIndex(e => e.SKU).IsUnique(); //SKU debe ser unico
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(50); //SKU debe ser unico y no puede superar los 50 caracteres

            entity.HasIndex(e => e.Barcode).IsUnique(false); //Barcode puede duplicarse
            entity.Property(e => e.Barcode).IsRequired().HasMaxLength(50);//Barcode puede duplicarse, pero no puede superar los 50 caracteres

            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Brand).IsRequired().HasMaxLength(100);

            //Clasificacion (Id para relacionarse 
            entity.Property(e => e.CategoryId).IsRequired();
            entity.Property(e => e.ProviderId).IsRequired();
            
            //Finanzas e Inventario
            entity.Property(e => e.NetCost).HasPrecision(18, 2);
            entity.Property(e => e.MarginPercentage).HasPrecision(5, 2);
            entity.Property(e => e.FinalPrice).HasPrecision(18, 2);
            entity.Property(e => e.Stock).IsRequired();
            
            //Auditoria y estado
            entity.Property(e => e.IsFavorite).HasDefaultValue(false);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

            //Opcionales
            entity.Property(e => e.WeightOrMeasure).HasMaxLength(50);
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.Model).HasMaxLength(100);
        });
    }
}


using Assessment_Empleabilidad.Domain.Entities;
using Assessment_Empleabilidad.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySqlConnector;

namespace Assessment_Empleabilidad.Infrastructure.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        // Configurar la clave primaria
        builder.HasKey(c => c.Id);
        
        // Configurar propiedades
        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("varchar(100)");
        
        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(c => c.IsDeleted)
            .IsRequired();
        
        builder.Property(c => c.CreatedAt)
            .IsRequired();
        
        builder.Property(c => c.UpdatedAt)
            .IsRequired(false);
        
        // Configurar la relación con Lesson
        builder.HasMany(c => c.Lessons)
            .WithOne(l => l.Course)
            .HasForeignKey(l => l.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
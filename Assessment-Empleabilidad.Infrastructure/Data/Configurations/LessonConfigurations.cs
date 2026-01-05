using Assessment_Empleabilidad.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySqlConnector;

namespace Assessment_Empleabilidad.Infrastructure.Data.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        // Configurar la clave primaria
        builder.HasKey(l => l.Id);
        
        // Configurar propiedades
        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("varchar(100)");
        
        builder.Property(l => l.Order)
            .IsRequired();
        
        builder.Property(l => l.IsDeleted)
            .IsRequired();
        
        builder.Property(l => l.CreatedAt)
            .IsRequired();
        
        builder.Property(l => l.UpdatedAt)
            .IsRequired(false);
        
        builder.Property(l => l.CourseId)
            .IsRequired();
        
        // Configurar la relación con Course
        builder.HasOne(l => l.Course)
            .WithMany(c => c.Lessons)
            .HasForeignKey(l => l.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
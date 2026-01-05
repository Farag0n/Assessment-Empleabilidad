using Assessment_Empleabilidad.Domain.Entities;
using Assessment_Empleabilidad.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Assessment_Empleabilidad.Infrastructure.Data.Configurations;

public class UserConfigurations :  IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        //valor único
        builder.HasIndex(u => u.Username).IsUnique();
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(100);
        
        //valor único
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .IsRequired();
        
        builder.Property(u => u.RefreshToken)
            .HasMaxLength(500);
        
        //Usuario admin de prueba 
        //No es una buena practica en entorno de produccion pero para pruebas en desarrollo creo que es valido
        builder.HasData(new User
        {
            Id = 1,
            Username = "test",
            Email = "test@qwe.com",
            Role = UserRole.Admin,
            PasswordHash = "123"
        });
    }
}
using EventMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventMaster.DAL.Extensions;

public static class ModelBuilderExtension
{
    public static void SeedUsersRolesData(this ModelBuilder modelBuilder)
    {
        var adminRoleId = new Guid("583E1840-BA88-418D-AE9E-4CE7571F0946");
        var adminId = new Guid("BD65E7BD-E25A-4935-81D1-05093B5F48C0");
        var adminPassword = "Admin14689";
        var adminRole = new Role()
        {
            Id = adminRoleId,
            IsDeleted = false,
            Name = "Admin"
        };
        var adminUser = new User()
        {
            Id = adminId,
            Login = "Admin",
            IsDeleted = false,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
            RefreshToken = "",
            RefreshTokenExpiryTime = DateTime.MinValue
        };
        modelBuilder.Entity<Role>().HasData(adminRole);
        modelBuilder.Entity<User>().HasData(adminUser);
        
        modelBuilder.Entity<Role>().HasData(new Role()
        {
            Id = Guid.NewGuid(),
            IsDeleted = false,
            Name = "Resident"
        });
        modelBuilder.Entity<UserRole>().HasData(new UserRole()
        {
            Id = Guid.NewGuid(),
            IsDeleted = false,
            RoleId = adminRoleId,
            UserId = adminId
        });
    }
}
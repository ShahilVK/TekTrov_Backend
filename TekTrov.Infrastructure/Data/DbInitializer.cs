using ECommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Domain.Entities;

namespace TekTrov.Infrastructure.Data
{
    public static async Task SeedAdminAsync(AppDbContext context)
    {
        var admin = context.Users.FirstOrDefault(u => u.Email == "admin@gmail.com");

        if (admin != null)
        {
            // 🔥 FORCE FIX EXISTING ADMIN ROLE
            admin.Role = "Admin";
            await context.SaveChangesAsync();
            return;
        }

        var newAdmin = new User
        {
            Name = "Admin",
            Email = "admin@gmail.com",
            Password = BCrypt.Net.BCrypt.HashPassword("Admin@123", 12),
            Role = "Admin",
            CreatedOn = DateTime.UtcNow
        };

        context.Users.Add(newAdmin);
        await context.SaveChangesAsync();
    }
}

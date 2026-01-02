using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekTrov.Domain.Entities;
using TekTrov.Domain.Enums;

namespace TekTrov.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAdminAsync(AppDbContext context)
        {
            var admin = await context.Users
                .FirstOrDefaultAsync(u => u.Email == "admin@gmail.com");

            if (admin == null)
            {
                admin = new User
                {
                    Name = "Admin",
                    Email = "admin@gmail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin@123", 12),
                    Role = "Admin",
                    CreatedOn = DateTime.UtcNow
                };

                context.Users.Add(admin);
            }
            else
            {
                // 🔥 FORCE ADMIN ROLE (fixes 403 forever)
                admin.Role = "Admin";
            }

            await context.SaveChangesAsync();
        }
    }
}


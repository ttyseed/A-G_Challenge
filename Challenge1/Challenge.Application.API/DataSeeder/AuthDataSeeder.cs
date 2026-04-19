using Microsoft.EntityFrameworkCore;
using challenge1.Application.Bll.Auth;
using challenge1.Database.Models;
using challenge1.Database.Repositories.Context;

namespace challenge1.Application.API.DataSeeder
{
    public static class AuthDataSeeder
    {
        public static async Task SeedAsync(DatabaseContext db)
        {
            if (await db.BackendUsers.AnyAsync()) return;

            db.BackendUsers.Add(new BackendUser
            {
                UserId        = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                LoginId       = "admin",
                FullName      = "Administrator",
                EmailAddress  = "admin@simsys.local",
                PasswordHash  = AuthBll.HashPassword("Admin@1234"),
                Status        = 1,
                IsDeleted     = false,
                CreatedById   = "SYSTEM",
                CreatedByName = "System",
                CreatedDate   = DateTime.Now
            });

            await db.SaveChangesAsync();
        }
    }
}

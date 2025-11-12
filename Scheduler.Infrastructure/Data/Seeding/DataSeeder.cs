using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scheduler.Domain.Entities;
using Scheduler.Infrastructure.Services;

namespace Scheduler.Infrastructure.Data.Seeding
{
    public class DataSeeder
    {
        private const string AdminUsername = "schedules.admin";
        private const string AdminEmail = "schedules.admin@example.com";
        private const string AdminPassword = "Scheduler@123";
        private const string AdminRoleName = "scheduler-admin";
        private const string AdminRoleDisplayName = "Scheduler Administrator";
        private const string AdminGroupName = "Scheduler Admin Group";
        private const string AdminGroupDescription = "Grupo padrão para o usuário schedules.admin.";

        private readonly AppDbContext _context;
        private readonly PasswordService _passwordService;

        public DataSeeder(AppDbContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            var now = DateTime.UtcNow;

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == AdminRoleName);
            if (role == null)
            {
                role = new Role
                {
                    Name = AdminRoleName,
                    DisplayName = AdminRoleDisplayName,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                await _context.Roles.AddAsync(role);
            }

            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Name == AdminGroupName);
            if (group == null)
            {
                group = new Group
                {
                    Name = AdminGroupName,
                    Description = AdminGroupDescription,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                await _context.Groups.AddAsync(group);
            }

            var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == AdminUsername);
            if (adminUser == null)
            {
                _passwordService.CreatePasswordHash(AdminPassword, out var hash, out var salt);

                adminUser = new User
                {
                    Username = AdminUsername,
                    Email = AdminEmail,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    CreatedAt = now,
                    UpdatedAt = now,
                    Role = role,
                    Group = group
                };

                await _context.Users.AddAsync(adminUser);
            }

            await _context.SaveChangesAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Scheduler.Domain.Entities;
using Scheduler.Domain.Interfaces;
using Scheduler.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scheduler.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Group)
                    .ThenInclude(g => g.PrimarySchedule)
                        .ThenInclude(s => s.ScheduleDays)
                            .ThenInclude(sd => sd.Weekday)
                .Include(u => u.Group)
                    .ThenInclude(g => g.SecondarySchedule)
                        .ThenInclude(s => s.ScheduleDays)
                            .ThenInclude(sd => sd.Weekday)
                .Include(u => u.Role)
                .Include(u => u.Manager)
                .Include(u => u.Subordinates)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Group)
                    .ThenInclude(g => g.PrimarySchedule)
                        .ThenInclude(s => s.ScheduleDays)
                            .ThenInclude(sd => sd.Weekday)
                .Include(u => u.Group)
                    .ThenInclude(g => g.SecondarySchedule)
                        .ThenInclude(s => s.ScheduleDays)
                            .ThenInclude(sd => sd.Weekday)
                .Include(u => u.Role)
                .Include(u => u.Manager)
                .Include(u => u.Subordinates)
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Group)
                    .ThenInclude(g => g.PrimarySchedule)
                        .ThenInclude(s => s.ScheduleDays)
                            .ThenInclude(sd => sd.Weekday)
                .Include(u => u.Group)
                    .ThenInclude(g => g.SecondarySchedule)
                        .ThenInclude(s => s.ScheduleDays)
                            .ThenInclude(sd => sd.Weekday)
                .Include(u => u.Role)
                .Include(u => u.Manager)
                .Include(u => u.Subordinates)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
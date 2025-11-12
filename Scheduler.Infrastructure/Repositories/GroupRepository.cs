using Microsoft.EntityFrameworkCore;
using Scheduler.Domain.Entities;
using Scheduler.Domain.Interfaces;
using Scheduler.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scheduler.Infrastructure.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly AppDbContext _context;

        public GroupRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Group?> GetByIdAsync(int id)
        {
            return await _context.Groups
                .Include(g => g.PrimarySchedule)
                    .ThenInclude(s => s.ScheduleDays)
                        .ThenInclude(sd => sd.Weekday)
                .Include(g => g.SecondarySchedule)
                    .ThenInclude(s => s.ScheduleDays)
                        .ThenInclude(sd => sd.Weekday)
                .Include(g => g.Users)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<List<Group>> GetAllAsync()
        {
            return await _context.Groups
                .Include(g => g.PrimarySchedule)
                .Include(g => g.SecondarySchedule)
                .Include(g => g.Users)
                .ToListAsync();
        }

        public async Task AddAsync(Group group)
        {
            await _context.Groups.AddAsync(group);
        }

        public void Update(Group group)
        {
            _context.Groups.Update(group);
        }

        public void Delete(Group group)
        {
            _context.Groups.Remove(group);
        }
    }
}
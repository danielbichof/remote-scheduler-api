using Microsoft.EntityFrameworkCore;
using Scheduler.Domain.Entities;
using Scheduler.Domain.Interfaces;
using Scheduler.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scheduler.Infrastructure.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly AppDbContext _context;

        public ScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Schedule?> GetByIdAsync(int id)
        {
            return await _context.Schedules
                .Include(s => s.ScheduleDays)
                    .ThenInclude(sd => sd.Weekday)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Schedule>> GetAllAsync()
        {
            return await _context.Schedules
                .Include(s => s.ScheduleDays)
                    .ThenInclude(sd => sd.Weekday)
                .ToListAsync();
        }

        public async Task AddAsync(Schedule schedule)
        {
            await _context.Schedules.AddAsync(schedule);
        }

        public void Update(Schedule schedule)
        {
            _context.Schedules.Update(schedule);
        }

        public void Delete(Schedule schedule)
        {
            _context.Schedules.Remove(schedule);
        }
    }
}
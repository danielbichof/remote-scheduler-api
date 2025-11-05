using Microsoft.EntityFrameworkCore;
using Scheduler.Domain.Entities;
using Scheduler.Domain.Interfaces;
using Scheduler.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scheduler.Infrastructure.Repositories
{
    public class PlannedScheduleRepository : IPlannedScheduleRepository
    {
        private readonly AppDbContext _context;

        public PlannedScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PlannedSchedule?> GetByIdAsync(int id)
        {
            return await _context.PlannedSchedules
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<PlannedSchedule>> GetAllAsync()
        {
            return await _context.PlannedSchedules
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task AddAsync(PlannedSchedule plannedSchedule)
        {
            await _context.PlannedSchedules.AddAsync(plannedSchedule);
        }

        public void Update(PlannedSchedule plannedSchedule)
        {
            _context.PlannedSchedules.Update(plannedSchedule);
        }

        public void Delete(PlannedSchedule plannedSchedule)
        {
            _context.PlannedSchedules.Remove(plannedSchedule);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Scheduler.Domain.Entities;
using Scheduler.Domain.Interfaces;
using Scheduler.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scheduler.Infrastructure.Repositories
{
    public class PlannedScheduleSolicitationRepository : IPlannedScheduleSolicitationRepository
    {
        private readonly AppDbContext _context;

        public PlannedScheduleSolicitationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PlannedScheduleSolicitation?> GetByIdAsync(int id)
        {
            return await _context.PlannedScheduleSolicitations.FindAsync(id);
        }

        public async Task<List<PlannedScheduleSolicitation>> GetAllAsync()
        {
            return await _context.PlannedScheduleSolicitations.ToListAsync();
        }

        public async Task AddAsync(PlannedScheduleSolicitation solicitation)
        {
            await _context.PlannedScheduleSolicitations.AddAsync(solicitation);
        }

        public void Update(PlannedScheduleSolicitation solicitation)
        {
            _context.PlannedScheduleSolicitations.Update(solicitation);
        }

        public void Delete(PlannedScheduleSolicitation solicitation)
        {
            _context.PlannedScheduleSolicitations.Remove(solicitation);
        }
    }
}
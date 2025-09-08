using Scheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Domain.Interfaces
{
    public interface IPlannedScheduleSolicitationRepository
    {
        Task<PlannedScheduleSolicitation?> GetByIdAsync(int id);
        Task<List<PlannedScheduleSolicitation>> GetAllAsync();
        Task AddAsync(PlannedScheduleSolicitation solicitation);
        void Update(PlannedScheduleSolicitation solicitation);
        void Delete(PlannedScheduleSolicitation solicitation);
    }
}

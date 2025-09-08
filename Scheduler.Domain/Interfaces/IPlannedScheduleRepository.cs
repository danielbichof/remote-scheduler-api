using Scheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Domain.Interfaces
{
    public interface IPlannedScheduleRepository
    {
        Task<PlannedSchedule?> GetByIdAsync(int id);
        Task<List<PlannedSchedule>> GetAllAsync();
        Task AddAsync(PlannedSchedule plannedSchedule);
        void Update(PlannedSchedule plannedSchedule);
        void Delete(PlannedSchedule plannedSchedule);
    }
}

using Scheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Domain.Interfaces
{
    public interface IScheduleRepository
    {
        Task<Schedule?> GetByIdAsync(int id);
        Task<List<Schedule>> GetAllAsync();
        Task AddAsync(Schedule schedule);
        void Update(Schedule schedule);
        void Delete(Schedule schedule);
    }
}

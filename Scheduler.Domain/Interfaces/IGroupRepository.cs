using Scheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Scheduler.Domain.Interfaces
{
    public interface IGroupRepository
    {
        Task<Group?> GetByIdAsync(int id);
        Task<List<Group>> GetAllAsync();
        Task AddAsync(Group group);
        void Update(Group group);
        void Delete(Group group);
    }
}

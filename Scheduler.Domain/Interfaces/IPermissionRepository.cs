using Scheduler.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scheduler.Domain.Interfaces
{
    public interface IPermissionRepository
    {
        Task<Permission?> GetByIdAsync(int id);
        Task<List<Permission>> GetAllAsync();
        Task AddAsync(Permission permission);
        void Update(Permission permission);
        void Delete(Permission permission);
    }
}

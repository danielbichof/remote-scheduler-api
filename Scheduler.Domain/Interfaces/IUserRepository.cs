using Scheduler.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scheduler.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<List<User>> GetAllAsync();     
        Task<User?> GetByEmailAsync(string email); 
        Task AddAsync(User user);
        void Update(User user);
        void Delete(User user);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scheduler.Application.DTOs;
using System.Threading.Tasks;

namespace Scheduler.Application.Services
{
    public interface IUserService
    {
        Task RegisterUserAsync(RegisterUserDto registerDto);
        Task<(int userId, string username, string email, int roleId)?> AuthenticateAsync(string email, string password);
    }
}

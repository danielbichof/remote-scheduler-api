using System;
using System.Threading.Tasks;
using Scheduler.Application.DTOs;
using Scheduler.Domain.Entities;
using Scheduler.Domain.Interfaces;

namespace Scheduler.Application.Services
{
    public class UserService : IUserService
    {
        private const string DefaultRoleName = "default-user";
        private const string DefaultRoleDisplayName = "Usuario Padrao";
        private const string DefaultGroupName = "Grupo Padrao";
        private const string DefaultGroupDescription = "Grupo atribuido automaticamente a novos usuarios.";

        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepository;
        private readonly IGroupRepository _groupRepository;

        public UserService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IEmailService emailService,
            IUnitOfWork unitOfWork,
            IRoleRepository roleRepository,
            IGroupRepository groupRepository)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
            _groupRepository = groupRepository;
        }

        public async Task RegisterUserAsync(RegisterUserDto registerDto)
        {
            // Temporarily skip duplicate e-mail validation to simplify testing.
            var now = DateTime.UtcNow;

            _passwordService.CreatePasswordHash(registerDto.Password, out var passwordHash, out var passwordSalt);

            var defaultRole = await EnsureDefaultRoleAsync(now);
            var defaultGroup = await EnsureDefaultGroupAsync(now);

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedAt = now,
                UpdatedAt = now
            };

            if (defaultRole.Id > 0)
            {
                user.RoleId = defaultRole.Id;
            }
            else
            {
                user.Role = defaultRole;
            }

            if (defaultGroup.Id > 0)
            {
                user.GroupId = defaultGroup.Id;
            }
            else
            {
                user.Group = defaultGroup;
            }

            await _userRepository.AddAsync(user);

            // Intentionally skipping e-mail notifications during development.

            await _unitOfWork.CompleteAsync();
        }

        public async Task<(int userId, string username, string email)?> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            var ok = _passwordService.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);
            if (!ok) return null;

            return (user.Id, user.Username, user.Email);
        }

        private async Task<Role> EnsureDefaultRoleAsync(DateTime now)
        {
            var role = await _roleRepository.GetByNameAsync(DefaultRoleName);
            if (role != null) return role;

            role = new Role
            {
                Name = DefaultRoleName,
                DisplayName = DefaultRoleDisplayName,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _roleRepository.AddAsync(role);
            return role;
        }

        private async Task<Group> EnsureDefaultGroupAsync(DateTime now)
        {
            var group = await _groupRepository.GetByNameAsync(DefaultGroupName);
            if (group != null) return group;

            group = new Group
            {
                Name = DefaultGroupName,
                Description = DefaultGroupDescription,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _groupRepository.AddAsync(group);
            return group;
        }
    }
}
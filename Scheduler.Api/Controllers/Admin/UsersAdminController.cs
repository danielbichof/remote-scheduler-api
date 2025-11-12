using Microsoft.AspNetCore.Mvc;
using Scheduler.Domain.Entities;
using Scheduler.Domain.Interfaces;
using Scheduler.Application.DTOs;
using Scheduler.Application.Services;
using System.Linq;

namespace Scheduler.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    public class UsersAdminController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;
        private readonly IRoleRepository _roleRepository;
        private readonly IGroupRepository _groupRepository;

        public UsersAdminController(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordService passwordService, IRoleRepository roleRepository, IGroupRepository groupRepository)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
            _roleRepository = roleRepository;
            _groupRepository = groupRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                // Retornar DTOs seguros (sem PasswordHash/PasswordSalt e sem relacionamentos circulares)
                var result = users.Select(u => new UserResponseDto
                { 
                    Id = u.Id, 
                    Username = u.Username ?? string.Empty, 
                    Email = u.Email ?? string.Empty, 
                    RoleId = u.RoleId, 
                    GroupId = u.GroupId, 
                    ManagerId = u.ManagerId,
                    CreatedAt = u.CreatedAt, 
                    UpdatedAt = u.UpdatedAt 
                }).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();
            
            // Retornar DTO seguro (sem PasswordHash/PasswordSalt e sem relacionamentos circulares)
            var response = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username ?? string.Empty,
                Email = user.Email ?? string.Empty,
                RoleId = user.RoleId,
                GroupId = user.GroupId,
                ManagerId = user.ManagerId,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
            
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminCreateUserDto dto)
        {
            // validate FKs to avoid 500 on DB constraints
            var role = await _roleRepository.GetByIdAsync(dto.RoleId);
            if (role == null) return BadRequest(new { message = "RoleId inválido" });
            var group = await _groupRepository.GetByIdAsync(dto.GroupId);
            if (group == null) return BadRequest(new { message = "GroupId inválido" });

            _passwordService.CreatePasswordHash(dto.Password, out var hash, out var salt);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                RoleId = dto.RoleId,
                GroupId = dto.GroupId,
                ManagerId = dto.ManagerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            // Retornar DTO seguro (sem PasswordHash, PasswordSalt e sem relacionamentos circulares)
            var response = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                RoleId = user.RoleId,
                GroupId = user.GroupId,
                ManagerId = user.ManagerId,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AdminUpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            // validate FKs to avoid 500 on DB constraints
            var role = await _roleRepository.GetByIdAsync(dto.RoleId);
            if (role == null) return BadRequest(new { message = "RoleId inválido" });
            var group = await _groupRepository.GetByIdAsync(dto.GroupId);
            if (group == null) return BadRequest(new { message = "GroupId inválido" });

            // Update only provided fields
            user.Username = dto.Username;
            user.Email = dto.Email;
            user.RoleId = dto.RoleId;
            user.GroupId = dto.GroupId;
            user.ManagerId = dto.ManagerId;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();
            _userRepository.Delete(user);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Scheduler.Domain.Entities;
using Scheduler.Domain.Interfaces;
using System.Linq;

namespace Scheduler.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/roles")]
    public class RolesAdminController : ControllerBase
    {
        private readonly IRoleRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public RolesAdminController(IRoleRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var roles = await _repo.GetAllAsync();
                // retorna apenas campos básicos para evitar referências circulares
                var result = roles.Select(r => new 
                { 
                    Id = r.Id, 
                    Name = r.Name ?? string.Empty, 
                    DisplayName = r.DisplayName 
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
            var r = await _repo.GetByIdAsync(id);
            if (r == null) return NotFound();
            // retorna apenas campos básicos
            return Ok(new { r.Id, r.Name, r.DisplayName });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Role role)
        {
            if (string.IsNullOrWhiteSpace(role.Name))
                return BadRequest(new { message = "Name é obrigatório" });

            role.CreatedAt = DateTime.UtcNow;
            role.UpdatedAt = DateTime.UtcNow;
            
            await _repo.AddAsync(role);
            await _unitOfWork.CompleteAsync();
            return CreatedAtAction(nameof(Get), new { id = role.Id }, new { role.Id, role.Name, role.DisplayName });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Role updated)
        {
            var r = await _repo.GetByIdAsync(id);
            if (r == null) return NotFound();
            r.Name = updated.Name;
            r.DisplayName = updated.DisplayName;
            r.UpdatedAt = DateTime.UtcNow;
            _repo.Update(r);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _repo.GetByIdAsync(id);
            if (r == null) return NotFound();
            _repo.Delete(r);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        // Role <-> Permission management
        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetPermissions(int id)
        {
            var r = await _repo.GetByIdAsync(id);
            if (r == null) return NotFound();
            return Ok(r.Permissions);
        }

        [HttpPost("{id}/permissions/{permissionId}")]
        public async Task<IActionResult> AddPermission(int id, int permissionId, [FromServices] IPermissionRepository permissionRepo)
        {
            var r = await _repo.GetByIdAsync(id);
            if (r == null) return NotFound();

            var p = await permissionRepo.GetByIdAsync(permissionId);
            if (p == null) return NotFound(new { message = "Permission not found" });

            if (!r.Permissions.Any(x => x.Id == permissionId))
            {
                r.Permissions.Add(p);
                _repo.Update(r);
                await _unitOfWork.CompleteAsync();
            }

            return NoContent();
        }

        [HttpDelete("{id}/permissions/{permissionId}")]
        public async Task<IActionResult> RemovePermission(int id, int permissionId, [FromServices] IPermissionRepository permissionRepo)
        {
            var r = await _repo.GetByIdAsync(id);
            if (r == null) return NotFound();

            var p = r.Permissions.FirstOrDefault(x => x.Id == permissionId);
            if (p == null) return NotFound(new { message = "Permission not associated with role" });

            r.Permissions.Remove(p);
            _repo.Update(r);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}

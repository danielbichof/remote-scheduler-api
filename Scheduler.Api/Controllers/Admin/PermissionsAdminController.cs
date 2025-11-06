using Microsoft.AspNetCore.Mvc;
using Scheduler.Domain.Entities;
using Scheduler.Domain.Interfaces;

namespace Scheduler.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/permissions")]
    public class PermissionsAdminController : ControllerBase
    {
        private readonly IPermissionRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public PermissionsAdminController(IPermissionRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return NotFound();
            return Ok(p);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Permission permission)
        {
            await _repo.AddAsync(permission);
            await _unitOfWork.CompleteAsync();
            return CreatedAtAction(nameof(Get), new { id = permission.Id }, permission);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Permission updated)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return NotFound();
            p.Name = updated.Name;
            p.DisplayName = updated.DisplayName;
            p.UpdatedAt = DateTime.UtcNow;
            _repo.Update(p);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return NotFound();
            _repo.Delete(p);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}

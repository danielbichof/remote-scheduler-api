using Microsoft.AspNetCore.Mvc;
using Scheduler.Domain.Entities;
using Scheduler.Domain.Interfaces;

namespace Scheduler.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/plannedschedules")]
    public class PlannedSchedulesAdminController : ControllerBase
    {
        private readonly IPlannedScheduleRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public PlannedSchedulesAdminController(IPlannedScheduleRepository repo, IUnitOfWork unitOfWork)
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
        public async Task<IActionResult> Create([FromBody] PlannedSchedule ps)
        {
            await _repo.AddAsync(ps);
            await _unitOfWork.CompleteAsync();
            return CreatedAtAction(nameof(Get), new { id = ps.Id }, ps);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PlannedSchedule updated)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return NotFound();
            p.WorkMode = updated.WorkMode;
            p.ScheduleDate = updated.ScheduleDate;
            p.UpdatedAt = DateTime.UtcNow;
            p.UserId = updated.UserId;
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

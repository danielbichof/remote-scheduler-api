using Microsoft.AspNetCore.Mvc;
using Scheduler.Domain.Entities;
using Scheduler.Domain.Interfaces;
using Scheduler.Application.DTOs;
using System.Linq;

namespace Scheduler.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/schedules")]
    public class SchedulesAdminController : ControllerBase
    {
        private readonly IScheduleRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public SchedulesAdminController(IScheduleRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var schedules = await _repo.GetAllAsync();
                // retorna campos básicos + dias para evitar referências circulares
                var result = schedules.Select(s => new 
                { 
                    Id = s.Id, 
                    Title = s.Title ?? string.Empty, 
                    Description = s.Description,
                    Days = s.ScheduleDays.Select(sd => new {
                        WeekdayId = sd.WeekdayId,
                        DayName = sd.Weekday != null ? sd.Weekday.DayName : null,
                        IsRemote = sd.IsRemote
                    }).ToList()
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
            var s = await _repo.GetByIdAsync(id);
            if (s == null) return NotFound();
            return Ok(new {
                Id = s.Id,
                Title = s.Title ?? string.Empty,
                Description = s.Description,
                Days = s.ScheduleDays.Select(sd => new {
                    WeekdayId = sd.WeekdayId,
                    DayName = sd.Weekday != null ? sd.Weekday.DayName : null,
                    IsRemote = sd.IsRemote
                }).ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminCreateScheduleDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { message = "Dados inválidos: corpo da requisição está vazio" });

                if (string.IsNullOrWhiteSpace(dto.Title))
                    return BadRequest(new { message = "Título é obrigatório" });

                var schedule = new Schedule
                {
                    Title = dto.Title.Trim(),
                    Description = dto.Description?.Trim() ?? null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Mapear dias, se enviados
                if (dto.Days != null && dto.Days.Count > 0)
                {
                    // Evitar duplicações de WeekdayId
                    var distinctDays = dto.Days
                        .Where(d => d != null)
                        .GroupBy(d => d.WeekdayId)
                        .Select(g => g.First());

                    foreach (var d in distinctDays)
                    {
                        schedule.ScheduleDays.Add(new ScheduleDay
                        {
                            WeekdayId = d.WeekdayId,
                            IsRemote = d.IsRemote,
                        });
                    }
                }
                
                await _repo.AddAsync(schedule);
                await _unitOfWork.CompleteAsync();
                return CreatedAtAction(nameof(Get), new { id = schedule.Id }, new { schedule.Id, schedule.Title, schedule.Description });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao criar escala: {ex.Message}", stackTrace = ex.StackTrace });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AdminCreateScheduleDto dto)
        {
            var s = await _repo.GetByIdAsync(id);
            if (s == null) return NotFound();
            
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest(new { message = "Título é obrigatório" });
            
            s.Title = dto.Title.Trim();
            s.Description = dto.Description?.Trim() ?? null;
            s.UpdatedAt = DateTime.UtcNow;

            // Atualiza dias, se enviados. Caso seja null, mantém os existentes.
            if (dto.Days != null)
            {
                // Limpa coleção atual para refletir substituição completa
                s.ScheduleDays.Clear();

                var distinctDays = dto.Days
                    .Where(d => d != null)
                    .GroupBy(d => d.WeekdayId)
                    .Select(g => g.First());

                foreach (var d in distinctDays)
                {
                    s.ScheduleDays.Add(new ScheduleDay
                    {
                        ScheduleId = s.Id,
                        WeekdayId = d.WeekdayId,
                        IsRemote = d.IsRemote,
                    });
                }
            }
            _repo.Update(s);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await _repo.GetByIdAsync(id);
            if (s == null) return NotFound();
            _repo.Delete(s);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}

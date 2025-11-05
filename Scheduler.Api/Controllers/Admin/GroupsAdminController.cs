using Microsoft.AspNetCore.Mvc;
using Scheduler.Domain.Entities;
using Scheduler.Domain.Interfaces;
using Scheduler.Application.DTOs;
using System.Linq;

namespace Scheduler.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/groups")]
    public class GroupsAdminController : ControllerBase
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GroupsAdminController(IGroupRepository groupRepository, IUnitOfWork unitOfWork)
        {
            _groupRepository = groupRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var groups = await _groupRepository.GetAllAsync();
                // retorna apenas campos básicos para evitar referências circulares
                var result = groups.Select(g => new 
                { 
                    Id = g.Id, 
                    Name = g.Name ?? string.Empty, 
                    Description = g.Description, 
                    PrimaryScheduleId = g.PrimaryScheduleId, 
                    SecondaryScheduleId = g.SecondaryScheduleId 
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
            var g = await _groupRepository.GetByIdAsync(id);
            if (g == null) return NotFound();
            // retorna apenas campos básicos
            return Ok(new { g.Id, g.Name, g.Description, g.PrimaryScheduleId, g.SecondaryScheduleId });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminCreateGroupDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { message = "Dados inválidos: corpo da requisição está vazio" });

                if (string.IsNullOrWhiteSpace(dto.Name))
                    return BadRequest(new { message = "Nome é obrigatório" });

                var group = new Group
                {
                    Name = dto.Name.Trim(),
                    Description = dto.Description?.Trim() ?? string.Empty,
                    PrimaryScheduleId = dto.PrimaryScheduleId,
                    SecondaryScheduleId = dto.SecondaryScheduleId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                await _groupRepository.AddAsync(group);
                await _unitOfWork.CompleteAsync();
                return CreatedAtAction(nameof(Get), new { id = group.Id }, new { group.Id, group.Name, group.Description, group.PrimaryScheduleId, group.SecondaryScheduleId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao criar grupo: {ex.Message}", stackTrace = ex.StackTrace });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AdminCreateGroupDto dto)
        {
            var g = await _groupRepository.GetByIdAsync(id);
            if (g == null) return NotFound();
            
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { message = "Nome é obrigatório" });
            
            g.Name = dto.Name;
            g.Description = dto.Description ?? string.Empty;
            g.PrimaryScheduleId = dto.PrimaryScheduleId;
            g.SecondaryScheduleId = dto.SecondaryScheduleId;
            g.UpdatedAt = DateTime.UtcNow;
            _groupRepository.Update(g);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var g = await _groupRepository.GetByIdAsync(id);
            if (g == null) return NotFound();
            _groupRepository.Delete(g);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}

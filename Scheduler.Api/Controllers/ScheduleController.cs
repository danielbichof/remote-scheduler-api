using Microsoft.AspNetCore.Mvc;
using Scheduler.Application.Services;
using System.Globalization;

namespace Scheduler.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IMonthlyScheduleService _monthlyScheduleService;

        public ScheduleController(IMonthlyScheduleService monthlyScheduleService)
        {
            _monthlyScheduleService = monthlyScheduleService;
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthly([FromQuery] int year, [FromQuery] int month, [FromQuery] int? userId)
        {
            if (year < 1 || month < 1 || month > 12)
                return BadRequest("Parâmetros inválidos: year e month são obrigatórios e month entre 1 e 12.");

            var schedule = await _monthlyScheduleService.GenerateForMonthAsync(year, month, userId);

            // Imprime no terminal (console) para debug/visualização rápida
            foreach (var item in schedule)
            {
                System.Console.WriteLine($"{item.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)} - {item.Username} - {item.WorkMode}");
            }

            return Ok(schedule);
        }
    }
}

using System.ComponentModel.DataAnnotations;
using Scheduler.Domain.Enums;

namespace Scheduler.Application.DTOs
{
    public class ScheduleDayInputDto
    {
        [Required]
        public int WeekdayId { get; set; }

        // WorkMode pode ser null (folga/indefinido), Remote (0), Office (1), ou Out (2)
        public WorkMode? WorkMode { get; set; }

        // Opcional: caso o cliente envie o nome
        public string? DayName { get; set; }
    }
}



using System.ComponentModel.DataAnnotations;

namespace Scheduler.Application.DTOs
{
    public class ScheduleDayInputDto
    {
        [Required]
        public int WeekdayId { get; set; }

        [Required]
        public bool IsRemote { get; set; }

        // Opcional: caso o cliente envie o nome
        public string? DayName { get; set; }
    }
}



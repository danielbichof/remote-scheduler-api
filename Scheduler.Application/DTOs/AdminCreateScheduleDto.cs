using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Scheduler.Application.DTOs
{
    public class AdminCreateScheduleDto
    {
        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        // Dias opcionais enviados no momento da criação/edição
        public List<ScheduleDayInputDto>? Days { get; set; }
    }
}


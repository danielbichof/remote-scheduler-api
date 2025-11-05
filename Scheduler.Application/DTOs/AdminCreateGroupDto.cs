using System.ComponentModel.DataAnnotations;

namespace Scheduler.Application.DTOs
{
    public class AdminCreateGroupDto
    {
        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public int? PrimaryScheduleId { get; set; }

        public int? SecondaryScheduleId { get; set; }
    }
}


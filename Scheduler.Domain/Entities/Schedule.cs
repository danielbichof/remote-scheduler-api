using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Domain.Entities
{
    public class Schedule
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string? Description { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<ScheduleDay> ScheduleDays { get; set; } = new List<ScheduleDay>();
        public virtual ICollection<Group> PrimaryGroups { get; set; } = new List<Group>();
        public virtual ICollection<Group> SecondaryGroups { get; set; } = new List<Group>();
    }
}


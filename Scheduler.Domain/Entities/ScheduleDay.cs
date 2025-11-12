using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler.Domain.Enums;

namespace Scheduler.Domain.Entities
{
    public class ScheduleDay
    {
        public int ScheduleId { get; set; }
        public int WeekdayId { get; set; }

        public WorkMode? WorkMode { get; set; } // null = folga/indefinido, Remote = 0, Office = 1, Out = 2

        public virtual Schedule Schedule { get; set; }
        public virtual Weekday Weekday { get; set; }

    }
}

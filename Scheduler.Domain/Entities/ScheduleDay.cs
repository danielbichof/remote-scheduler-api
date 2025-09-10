using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Domain.Entities
{
    public class ScheduleDay
    {
        public int ScheduleId { get; set; }
        public int WeekdayId { get; set; }

        public bool IsRemote { get; set; }

        public virtual Schedule Schedule { get; set; }
        public virtual Weekday Weekday { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Domain.Entities
{
    public class Weekday
    {
        public int Id { get; set; }
        public string DayName { get; set; }

        public virtual ICollection<ScheduleDay> ScheduleDays { get; set; } = new List<ScheduleDay>();

    }
}

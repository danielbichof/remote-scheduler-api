using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Domain.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int? PrimaryScheduleId { get; set; }
        public virtual Schedule PrimarySchedule { get; set; }

        public int? SecondaryScheduleId { get; set; }
        public virtual Schedule SecondarySchedule { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}

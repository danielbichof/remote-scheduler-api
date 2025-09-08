using Scheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Domain.Entities
{
    public class PlannedScheduleSolicitation
    {
        public int Id { get; set; }
        public DateTime PlannedScheduleDate { get; set; }
        public WorkMode WorkMode { get; set; }
        public WorkMode PreviousWorkMode { get; set; }
        public string? Comment { get; set; }
        public string? ManagerReply { get; set; }
        public DateTime CreatedAt { get; set; }

        public int RequestedById { get; set; }
        public virtual User RequestedBy { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int? ReviewerId { get; set; }
        public virtual User? Reviewer { get; set; }
    }
}

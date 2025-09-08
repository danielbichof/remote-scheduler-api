using Scheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Domain.Entities
{
    public class PlannedSchedule
    {
        public int Id { get; set; }
        public WorkMode WorkMode { get; set; }

        // Colunas de Dados
        public DateTime ScheduleDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

    }
}

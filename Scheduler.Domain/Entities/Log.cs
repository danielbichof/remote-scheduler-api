using Scheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Domain.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public LogLevel Level { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

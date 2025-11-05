using Scheduler.Domain.Enums;
using System;

namespace Scheduler.Application.DTOs
{
    public class MonthlyScheduleDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public WorkMode WorkMode { get; set; }
    }
}

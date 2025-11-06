using Scheduler.Application.DTOs;
using Scheduler.Domain.Entities;
using Scheduler.Domain.Enums;
using Scheduler.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Application.Services
{
    public class MonthlyScheduleService : IMonthlyScheduleService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;

        public MonthlyScheduleService(IUserRepository userRepository, IGroupRepository groupRepository)
        {
            _userRepository = userRepository;
            _groupRepository = groupRepository;
        }

        public async Task<List<MonthlyScheduleDto>> GenerateForMonthAsync(int year, int month, int? userId = null)
        {
            var users = new List<User>();
            if (userId.HasValue)
            {
                var u = await _userRepository.GetByIdAsync(userId.Value);
                if (u != null) users.Add(u);
            }
            else
            {
                users = await _user_repository_getall_async();
            }

            var results = new List<MonthlyScheduleDto>();

            var daysInMonth = DateTime.DaysInMonth(year, month);

            // Determine week index for each date (ISO-like: week starts on Monday)
            // We'll flip patterns each calendar week.

            // Precompute a mapping date->weekIndex (0-based)
            var weekIndexByDate = new Dictionary<DateTime, int>();
            int currentWeek = -1;
            DateTime? lastMonday = null;
            for (int d = 1; d <= daysInMonth; d++)
            {
                var date = new DateTime(year, month, d);
                // Find Monday of the week containing this date
                var monday = date;
                while (monday.DayOfWeek != DayOfWeek.Monday)
                    monday = monday.AddDays(-1);

                if (!lastMonday.HasValue || monday != lastMonday.Value)
                {
                    currentWeek++;
                    lastMonday = monday;
                }

                weekIndexByDate[date] = currentWeek;
            }

            foreach (var user in users)
            {
                var groupName = user.Group?.Name?.Trim()?.ToUpperInvariant() ?? "A"; // default A

                for (int d = 1; d <= daysInMonth; d++)
                {
                    var date = new DateTime(year, month, d);

                    // skip weekends
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                        continue;

                    var weekIdx = weekIndexByDate[date];

                    // For weekIdx even -> base pattern, weekIdx odd -> swapped patterns
                    var isSwappedWeek = (weekIdx % 2 != 0);

                    // Base pattern (not swapped):
                    // Group A: presencial Tue/Thu, remoto other weekdays (Mon, Wed, Fri)
                    // Group B: inverse - presencial Mon/Wed/Fri, remoto Tue/Thu

                    WorkMode mode;

                    // Determine effective group for this week: if swapped, A<->B
                    var effectiveGroup = groupName;
                    if (isSwappedWeek)
                    {
                        if (groupName == "A") effectiveGroup = "B";
                        else if (groupName == "B") effectiveGroup = "A";
                    }

                    if (effectiveGroup == "A")
                    {
                        // A presencial Tue/Thu
                        if (date.DayOfWeek == DayOfWeek.Tuesday || date.DayOfWeek == DayOfWeek.Thursday)
                            mode = WorkMode.Office;
                        else
                            mode = WorkMode.Remote;
                    }
                    else // B
                    {
                        // B does the opposite
                        if (date.DayOfWeek == DayOfWeek.Tuesday || date.DayOfWeek == DayOfWeek.Thursday)
                            mode = WorkMode.Remote;
                        else
                            mode = WorkMode.Office;
                    }

                    results.Add(new MonthlyScheduleDto
                    {
                        UserId = user.Id,
                        Username = user.Username,
                        Date = date,
                        WorkMode = mode
                    });
                }
            }

            return results.OrderBy(r => r.UserId).ThenBy(r => r.Date).ToList();
        }

        // Helper to call repository GetAllAsync safe (keeps code readable)
        private async Task<List<User>> _user_repository_getall_async()
        {
            return await _userRepository.GetAllAsync();
        }
    }
}

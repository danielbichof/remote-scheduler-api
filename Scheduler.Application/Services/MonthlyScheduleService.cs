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

            foreach (var user in users)
            {
                if (user.Group == null) continue;

                // Carrega o grupo completo com as escalas e seus dias
                var group = await _groupRepository.GetByIdAsync(user.Group.Id);
                if (group == null) continue;

                // Busca as escalas primária e secundária do grupo
                var primarySchedule = group.PrimarySchedule;
                var secondarySchedule = group.SecondarySchedule;
                
                // Debug: log das escalas encontradas
                System.Console.WriteLine($"[DEBUG] User {user.Id} ({user.Username}) - Group: {group.Id}");
                System.Console.WriteLine($"[DEBUG] PrimarySchedule: {(primarySchedule != null ? $"Id={primarySchedule.Id}, Title={primarySchedule.Title}, Days={primarySchedule.ScheduleDays?.Count ?? 0}" : "NULL")}");
                System.Console.WriteLine($"[DEBUG] SecondarySchedule: {(secondarySchedule != null ? $"Id={secondarySchedule.Id}, Title={secondarySchedule.Title}, Days={secondarySchedule.ScheduleDays?.Count ?? 0}" : "NULL")}");
                
                // Se não tiver nenhuma escala, pula o usuário
                if ((primarySchedule == null || primarySchedule.ScheduleDays == null || !primarySchedule.ScheduleDays.Any()) &&
                    (secondarySchedule == null || secondarySchedule.ScheduleDays == null || !secondarySchedule.ScheduleDays.Any()))
                {
                    System.Console.WriteLine($"[DEBUG] User {user.Id} pulado - nenhuma escala disponível");
                    continue;
                }

                // Mapeia os dias da semana das escalas (WeekdayId -> WorkMode)
                var primaryScheduleDaysMap = primarySchedule?.ScheduleDays
                    ?.Where(sd => sd.WorkMode.HasValue)
                    .ToDictionary(sd => sd.WeekdayId, sd => sd.WorkMode.Value) ?? new Dictionary<int, WorkMode>();

                var secondaryScheduleDaysMap = secondarySchedule?.ScheduleDays
                    ?.Where(sd => sd.WorkMode.HasValue)
                    .ToDictionary(sd => sd.WeekdayId, sd => sd.WorkMode.Value) ?? new Dictionary<int, WorkMode>();

                System.Console.WriteLine($"[DEBUG] PrimaryScheduleDaysMap: {primaryScheduleDaysMap.Count} dias");
                System.Console.WriteLine($"[DEBUG] SecondaryScheduleDaysMap: {secondaryScheduleDaysMap.Count} dias");

                // Calcula a primeira segunda-feira do mês (semana que contém o dia 1)
                var firstDayOfMonth = new DateTime(year, month, 1);
                var firstMonday = firstDayOfMonth;
                while (firstMonday.DayOfWeek != DayOfWeek.Monday)
                    firstMonday = firstMonday.AddDays(-1);

                for (int d = 1; d <= daysInMonth; d++)
                {
                    var date = new DateTime(year, month, d);
                    var weekdayId = GetWeekdayId(date.DayOfWeek);
                    
                    // Encontra a segunda-feira da semana que contém esta data
                    var mondayOfWeek = date;
                    while (mondayOfWeek.DayOfWeek != DayOfWeek.Monday)
                        mondayOfWeek = mondayOfWeek.AddDays(-1);
                    
                    // Calcula o número da semana baseado na diferença de dias desde a primeira segunda-feira
                    // A primeira semana (que contém o dia 1) sempre será 0
                    var daysDiff = (mondayOfWeek - firstMonday).TotalDays;
                    var weekNumber = (int)Math.Floor(daysDiff / 7);
                    
                    // Semana 0 (primeira semana do mês) = Primary, depois alterna
                    // 0 = Primary, 1 = Secondary, 2 = Primary, 3 = Secondary, etc.
                    var usePrimarySchedule = weekNumber % 2 == 0;

                    // Debug: log da decisão de escala (apenas para alguns dias para não poluir muito)
                    if (d <= 3 || d == 8 || d == 15 || d == 22)
                    {
                        System.Console.WriteLine($"[DEBUG] Data: {date:yyyy-MM-dd}, WeekNumber: {weekNumber}, UsePrimary: {usePrimarySchedule}, MondayOfWeek: {mondayOfWeek:yyyy-MM-dd}");
                    }

                    // Seleciona a escala baseado na semana
                    Dictionary<int, WorkMode> scheduleDaysMap;
                    if (usePrimarySchedule)
                    {
                        // Usa Primary se disponível, senão usa Secondary como fallback
                        scheduleDaysMap = primaryScheduleDaysMap.Any() ? primaryScheduleDaysMap : secondaryScheduleDaysMap;
                    }
                    else
                    {
                        // Usa Secondary se disponível, senão usa Primary como fallback
                        scheduleDaysMap = secondaryScheduleDaysMap.Any() ? secondaryScheduleDaysMap : primaryScheduleDaysMap;
                    }

                    // Se não tiver nenhuma escala disponível, pula este dia
                    if (!scheduleDaysMap.Any())
                        continue;

                    // Busca o WorkMode da escala para este dia da semana
                    if (scheduleDaysMap.TryGetValue(weekdayId, out var workMode))
                    {
                        results.Add(new MonthlyScheduleDto
                        {
                            UserId = user.Id,
                            Username = user.Username,
                            Date = date,
                            WorkMode = workMode
                        });
                    }
                    // Se não encontrar na escala (null), não adiciona evento (folga/indefinido)
                }
            }

            return results.OrderBy(r => r.UserId).ThenBy(r => r.Date).ToList();
        }

        // Helper to call repository GetAllAsync safe (keeps code readable)
        private async Task<List<User>> _user_repository_getall_async()
        {
            return await _userRepository.GetAllAsync();
        }

        // Converte DayOfWeek para WeekdayId (1=Segunda, 2=Terça, ..., 7=Domingo)
        private int GetWeekdayId(DayOfWeek dayOfWeek)
        {
            // C# DayOfWeek: Sunday=0, Monday=1, ..., Saturday=6
            // Nosso WeekdayId: Monday=1, Tuesday=2, ..., Sunday=7
            return dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
        }
    }
}

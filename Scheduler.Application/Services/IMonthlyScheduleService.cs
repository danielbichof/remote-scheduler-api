using Scheduler.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scheduler.Application.Services
{
    public interface IMonthlyScheduleService
    {
        /// <summary>
        /// Gera uma lista de registros de escala para o usuário informado no mês/ano.
        /// Se userId for null, gera para todos os usuários.
        /// </summary>
        Task<List<MonthlyScheduleDto>> GenerateForMonthAsync(int year, int month, int? userId = null);
    }
}

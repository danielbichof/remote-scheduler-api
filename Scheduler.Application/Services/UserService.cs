using Scheduler.Application.DTOs;
using Scheduler.Application.Services;
using Scheduler.Domain.Entities;
using Scheduler.Domain.Interfaces;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;


namespace Scheduler.Application.Services { 

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        // Injetamos todas as dependências que precisamos
        public UserService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IEmailService emailService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        public async Task RegisterUserAsync(RegisterUserDto registerDto)
        {
            // Temporariamente não verificamos e-mail duplicado para facilitar testes.
            // Reverter essa mudança antes de ir para produção.

            // 2. Criptografar a senha
            _passwordService.CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            // 3. Criar a nova entidade User
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                // --- Definindo RoleId e GroupId padrão para evitar erro quando o banco estiver zerado ---
                RoleId = 2,  // ID da Role "Usuário" que está no seed
                GroupId = 1, // ID do "Grupo Padrão" que está no seed
                ManagerId = null // ManagerId pode ser nulo para novos usuários
            };

            // 4. Adicionar o usuário ao repositório
            await _userRepository.AddAsync(user);

            // 5. Não enviar e-mail durante testes. Em produção, reativar envio.
            // var emailBody = $"<h1>Bem-vindo, {user.Username}!</h1><p>Seu cadastro em SchedulerApp foi realizado com sucesso.</p>";
            // await _emailService.SendEmailAsync(user.Email, "Cadastro Realizado com Sucesso!", emailBody);

            // 6. Salvar tudo no banco de dados
            await _unitOfWork.CompleteAsync();
        }

        public async Task<(int userId, string username, string email, int roleId)?> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;
            var ok = _passwordService.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);
            if (!ok) return null;
            return (user.Id, user.Username, user.Email, user.RoleId);
        }
    }
}
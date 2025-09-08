using Microsoft.EntityFrameworkCore;
using Scheduler.Domain.Interfaces;
using Scheduler.Infrastructure.Data;
using Scheduler.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// --- Adicionando serviços ao container ---

// Serviços do template padrão e para controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Configuração da Connection String e DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString,
        b => b.MigrationsAssembly("Scheduler.Infrastructure")));

// 2. Registro dos Repositórios e da Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPlannedScheduleRepository, PlannedScheduleRepository>();
builder.Services.AddScoped<IPlannedScheduleSolicitationRepository, PlannedScheduleSolicitationRepository>();

// --- Construção da aplicação ---
var app = builder.Build();

// --- Configuração do pipeline de requisições HTTP ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Mapeia os controllers para que as rotas funcionem
app.MapControllers();

// Inicia a aplicação
app.Run();
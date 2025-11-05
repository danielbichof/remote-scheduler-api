using Microsoft.EntityFrameworkCore;
using Scheduler.Domain.Interfaces;
using Scheduler.Infrastructure.Data;
using Scheduler.Infrastructure.Repositories;
using Scheduler.Application.Services;
using Scheduler.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS: allow frontend dev server
var allowedClientOrigin = builder.Configuration.GetValue<string>("ClientOrigin") ?? "http://localhost:5173";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
        policy.WithOrigins(allowedClientOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString,
        b => b.MigrationsAssembly("Scheduler.Infrastructure")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPlannedScheduleRepository, PlannedScheduleRepository>();
builder.Services.AddScoped<IPlannedScheduleSolicitationRepository, PlannedScheduleSolicitationRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IMonthlyScheduleService, MonthlyScheduleService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// In development, avoid forcing HTTPS to simplify local frontend -> API calls
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowClient");

app.MapControllers();

app.Run();
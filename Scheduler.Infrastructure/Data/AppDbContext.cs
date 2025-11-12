using Microsoft.EntityFrameworkCore;
using Scheduler.Domain.Entities;

// O namespace reflete a nova pasta que criamos.
namespace Scheduler.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        // Este construtor � essencial para a Inje��o de Depend�ncia funcionar.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // --- Mapeamento das Entidades para Tabelas (DbSets) ---
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ScheduleDay> ScheduleDays { get; set; }
        public DbSet<Weekday> Weekdays { get; set; }
        public DbSet<PlannedSchedule> PlannedSchedules { get; set; }
        public DbSet<PlannedScheduleSolicitation> PlannedScheduleSolicitations { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Log> Logs { get; set; }


        // --- Configura��es Finas dos Relacionamentos (Fluent API) ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ======================================================================
            // --- 1. DATA SEEDING (Dados Iniciais) ---
            // Inserimos os dados m�nimos para o sistema funcionar.
            // ======================================================================

            // Roles Essenciais
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", DisplayName = "Administrador" },
                new Role { Id = 2, Name = "User", DisplayName = "Usu�rio" }
            );

            // Dias da Semana (Necess�rio para Schedules)
            modelBuilder.Entity<Weekday>().HasData(
                new Weekday { Id = 1, DayName = "Domingo" },
                new Weekday { Id = 2, DayName = "Segunda-feira" },
                new Weekday { Id = 3, DayName = "Ter�a-feira" },
                new Weekday { Id = 4, DayName = "Quarta-feira" },
                new Weekday { Id = 5, DayName = "Quinta-feira" },
                new Weekday { Id = 6, DayName = "Sexta-feira" },
                new Weekday { Id = 7, DayName = "S�bado" }
            );

            // Hor�rio Padr�o (Depend�ncia para o Grupo Padr�o)
            // Seus avisos de compila��o (CS8618) para Group indicam que
            // PrimarySchedule e SecondarySchedule n�o podem ser nulos.
            modelBuilder.Entity<Schedule>().HasData(
                new Schedule { Id = 1, Title = "Hor�rio Padr�o", Description = "Hor�rio de fallback inicial." }
            );

            // Grupo Padr�o (Resolve o erro FK_Users_Groups_GroupId)
            modelBuilder.Entity<Group>().HasData(
                new Group
                {
                    Id = 1,
                    Name = "Grupo Padr�o",
                    Description = "Grupo inicial para novos usu�rios",
                    PrimaryScheduleId = 1,  // Linkado ao Schedule(Id=1)
                    SecondaryScheduleId = 1 // Linkado ao Schedule(Id=1)
                }
            );

            // ======================================================================
            // --- 2. CONFIGURA��ES DE RELACIONAMENTO (Fluent API) ---
            // ======================================================================

            // --- Configura��o para a entidade User ---
            modelBuilder.Entity<User>(entity =>
            {
                // Configura o auto-relacionamento de Manager/Subordinates
                entity.HasOne(u => u.Manager)
                      .WithMany(u => u.Subordinates)
                      .HasForeignKey(u => u.ManagerId)
                      .OnDelete(DeleteBehavior.Restrict); // Impede que um gerente seja deletado se tiver subordinados.
            });

            // --- Configura��o para a entidade Group ---
            modelBuilder.Entity<Group>(entity =>
            {
                // Configura as DUAS chaves estrangeiras para a mesma tabela Schedule
                entity.HasOne(g => g.PrimarySchedule)
                      .WithMany(s => s.PrimaryGroups) // Usa a propriedade de navega��o inversa correta
                      .HasForeignKey(g => g.PrimaryScheduleId)
                      .OnDelete(DeleteBehavior.SetNull); // Ex: Se a escala for deletada, o campo no grupo fica nulo.

                entity.HasOne(g => g.SecondarySchedule)
                      .WithMany(s => s.SecondaryGroups) // Usa a outra propriedade de navega��o
                      .HasForeignKey(g => g.SecondaryScheduleId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // --- Configura��o para a entidade ScheduleDay ---
            modelBuilder.Entity<ScheduleDay>(entity =>
            {
                // Define a Chave Prim�ria Composta (PK composta por duas colunas)
                entity.HasKey(sd => new { sd.ScheduleId, sd.WeekdayId });
            });

            // --- Configura��o para Role e Permission (Muitos-para-Muitos) ---
            modelBuilder.Entity<Role>(entity =>
            {
                // Configura a rela��o muitos-para-muitos com Permission.
                // O EF Core criar� a tabela de jun��o 'RolePermission' automaticamente.
                entity.HasMany(r => r.Permissions)
                      .WithMany(p => p.Roles);
            });

            // --- Configura��o para PlannedScheduleSolicitation (M�ltiplas FKs para User) ---
            modelBuilder.Entity<PlannedScheduleSolicitation>(entity =>
            {
                // Ensina ao EF Core qual propriedade de navega��o corresponde a qual chave estrangeira

                // 1. Rela��o para o Solicitante (RequestedBy)
                entity.HasOne(s => s.RequestedBy)
                      .WithMany() // N�o h� cole��o inversa em User para esta rela��o
                      .HasForeignKey(s => s.RequestedById)
                      .OnDelete(DeleteBehavior.Restrict);

                // 2. Rela��o para o Usu�rio (User)
                entity.HasOne(s => s.User)
                      .WithMany()
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // 3. Rela��o para o Revisor (Reviewer)
                entity.HasOne(s => s.Reviewer)
                      .WithMany()
                      .HasForeignKey(s => s.ReviewerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
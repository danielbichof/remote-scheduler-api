using Microsoft.EntityFrameworkCore;
using Scheduler.Domain.Entities;

// O namespace reflete a nova pasta que criamos.
namespace Scheduler.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        // Este construtor é essencial para a Injeção de Dependência funcionar.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // --- Mapeamento das Entidades para Tabelas (DbSets) ---
        // Para cada entidade que você quer que vire uma tabela, adicione um DbSet.
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


        // --- Configurações Finas dos Relacionamentos (Fluent API) ---
        // Este método é onde ensinamos ao EF Core as regras complexas do nosso modelo.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configuração para a entidade User ---
            modelBuilder.Entity<User>(entity =>
            {
                // Configura o auto-relacionamento de Manager/Subordinates
                entity.HasOne(u => u.Manager)
                      .WithMany(u => u.Subordinates)
                      .HasForeignKey(u => u.ManagerId)
                      .OnDelete(DeleteBehavior.Restrict); // Impede que um gerente seja deletado se tiver subordinados.
            });

            // --- Configuração para a entidade Group ---
            modelBuilder.Entity<Group>(entity =>
            {
                // Configura as DUAS chaves estrangeiras para a mesma tabela Schedule
                entity.HasOne(g => g.PrimarySchedule)
                      .WithMany(s => s.PrimaryGroups) // Usa a propriedade de navegação inversa correta
                      .HasForeignKey(g => g.PrimaryScheduleId)
                      .OnDelete(DeleteBehavior.SetNull); // Ex: Se a escala for deletada, o campo no grupo fica nulo.

                entity.HasOne(g => g.SecondarySchedule)
                      .WithMany(s => s.SecondaryGroups) // Usa a outra propriedade de navegação
                      .HasForeignKey(g => g.SecondaryScheduleId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // --- Configuração para a entidade ScheduleDay ---
            modelBuilder.Entity<ScheduleDay>(entity =>
            {
                // Define a Chave Primária Composta (PK composta por duas colunas)
                entity.HasKey(sd => new { sd.ScheduleId, sd.WeekdayId });
            });

            // --- Configuração para Role e Permission (Muitos-para-Muitos) ---
            modelBuilder.Entity<Role>(entity =>
            {
                // Configura a relação muitos-para-muitos com Permission.
                // O EF Core criará a tabela de junção 'RolePermission' automaticamente.
                entity.HasMany(r => r.Permissions)
                      .WithMany(p => p.Roles);
            });

            // --- Configuração para PlannedScheduleSolicitation (Múltiplas FKs para User) ---
            modelBuilder.Entity<PlannedScheduleSolicitation>(entity =>
            {
                // Ensina ao EF Core qual propriedade de navegação corresponde a qual chave estrangeira

                // 1. Relação para o Solicitante (RequestedBy)
                entity.HasOne(s => s.RequestedBy)
                      .WithMany() // Não há coleção inversa em User para esta relação
                      .HasForeignKey(s => s.RequestedById)
                      .OnDelete(DeleteBehavior.Restrict);

                // 2. Relação para o Usuário (User)
                entity.HasOne(s => s.User)
                      .WithMany()
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // 3. Relação para o Revisor (Reviewer)
                entity.HasOne(s => s.Reviewer)
                      .WithMany()
                      .HasForeignKey(s => s.ReviewerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
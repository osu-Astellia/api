using Microsoft.EntityFrameworkCore;

namespace AstelliaAPI.Database
{
    public sealed class AstelliaDbContext : DbContext
    {
        public AstelliaDbContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Clan> Clans { get; set; }
        public DbSet<UserClan> UserClans { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<PPGraph> PPGraphs { get; set; }
        public DbSet<UpdaterInfo> UpdaterInfo { get; set; }
        public DbSet<UserStats> UsersStats { get; set; }
        public DbSet<RelaxStats> RelaxStats { get; set; }
        public DbSet<ScoreRelax> ScoresRelax { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Beatmap> Beatmaps { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Supporters> Supporters { get; set; }

        public DbSet<UsernameHistory> UsernameHistories { get; set; }
        public DbSet<Bills> Bills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<UserClan>(eb => { eb.HasNoKey(); }).Entity<PPGraph>(eb => { eb.HasNoKey(); })
                .Entity<UpdaterInfo>(eb => { eb.HasNoKey(); });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                $"server=localhost;database={Config.Get().Database};user={Config.Get().Username};password={Config.Get().Password}");
        }
    }
}
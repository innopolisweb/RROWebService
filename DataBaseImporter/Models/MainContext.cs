using Microsoft.EntityFrameworkCore;

namespace DataBaseImporter.Models
{
    public class MainContext : DbContext
    {
        public MainContext() : base(new DbContextOptions<MainContext>())
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Properties.Resources.ConnectionString);
        }

        public DbSet<RROTeam> Teams { get; set; }

        public DbSet<RROJudgeCv> JudgesCv { get; set; }

        public DbSet<RROJudgeFin> JudgesFin { get; set; }

        public DbSet<OmlScore> OMLScoreBoard { get; set; }

        public DbSet<CurrentRound> CurrentRound { get; set; }
    }
}
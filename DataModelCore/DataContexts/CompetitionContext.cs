using System;
using DataModelCore.ObjectModel;
using DataModelCore.ObjectModel.Primitives;
using Microsoft.EntityFrameworkCore;

namespace DataModelCore.DataContexts
{
    public class CompetitionContext : DbContext
    {
        public CompetitionContext() : base(new DbContextOptions<CompetitionContext>())
        {
             

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Resources.ResourceManager.GetString("ConnectionString") ?? throw new InvalidOperationException());
        }

        public DbSet<RROTeamCv> TeamsCv { get; set; }

        public DbSet<RROTeamFin> TeamsFin { get; set; }

        public DbSet<RROJudgeCv> JudgesCv { get; set; }

        public DbSet<RROJudgeFin> JudgesFin { get; set; }
                                                                
        public DbSet<OmlScore> OMLPreResults { get; set; }

        public DbSet<CompetitionRound> Rounds { get; set; }

        public DbSet<CurrentTour> CurrentTour { get; set; }

        public DbSet<Service> Services { get; set; }
    }
}
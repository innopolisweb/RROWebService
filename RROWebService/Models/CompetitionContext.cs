using DataModelCore.ObjectModel;
using DataModelCore.ObjectModel.Primitives;
using Microsoft.EntityFrameworkCore;

namespace RROWebService.Models
{
    public class CompetitionContext : DbContext
    {
        public CompetitionContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<RROTeamCv> TeamsCv { get; set; }

        public DbSet<RROTeamFin> TeamsFin { get; set; }

        public DbSet<RROJudgeCv> JudgesCv { get; set; }

        public DbSet<RROJudgeFin> JudgesFin { get; set; }

        public DbSet<OmlScore> OMLScoreBoard { get; set; }

        public DbSet<CurrentRound> CurrentRound { get; set; }

        public DbSet<CurrentTour> CurrentTour { get; set; }
    }
}
using Microsoft.EntityFrameworkCore;
using RROWebService.Models.ObjectModel;
using RROWebService.Models.ObjectModel.Primitives;

namespace RROWebService.Models
{
    public class CompetitionContext : DbContext
    {
        public CompetitionContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<RROTeam> Teams { get; set; }

        public DbSet<RROJudge> Judges { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<OmlScore> OMLScoreBoard { get; set; }

        public DbSet<CurrentRound> CurrentRound { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace DataModelCore.ObjectModel.Primitives
{
    public class CompetitionRound
    {
        [Key]
        public int Node { get; set; }
        public int Round { get; set; }
        public string Category { get; set; }
        public int Current { get; set; }
    }
}
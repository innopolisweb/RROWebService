using System.ComponentModel.DataAnnotations;

namespace DataModelCore.ObjectModel
{
    public class OmlScore
    {
        [Key]
        public int Node { get; set; }

        public string TeamId { get; set; }

        public int Round { get; set; }

        public string JudgeId { get; set; }

        public int Polygon { get; set; }

        public int? StaysCorrectly { get; set; }

        public int? LiesCorrectly { get; set; }

        public int? PartiallyCorrect { get; set; }

        public int? StaysIncorrectly { get; set; }

        public int? LiesIncorrectly { get; set; }

        public int? None { get; set; }

        public int? BlueBlockState { get; set; }

        public int? BlackBlockState { get; set; }

        public int? FinishCorrectly { get; set; }

        public int? BrokenWall { get; set; }

        public int? TimeMils { get; set; }

        public int Saved { get; set; }

    }
}
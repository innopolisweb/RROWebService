namespace RROWebService.Models.Categories.Oml
{
    public class OmlScoreViewModel
    {
        public string TeamId { get; set; }

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
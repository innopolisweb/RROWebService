using System.ComponentModel.DataAnnotations;

namespace DataModelCore.ObjectModel
{
    public class OmlScore
    {
        [Key]
        public int Node { get; set; }

        public string TeamId { get; set; }

        public int Tour { get; set; }

        public int Round { get; set; }

        public string JudgeId { get; set; }

        public int Polygon { get; set; }

        public int? RedBlockState { get; set; }

        public int? YellowBlockState { get; set; }

        public int? GreenBlockState { get; set; }

        public int? WhiteBlock1State { get; set; }

        public int? WhiteBlock2State { get; set; }

        public int? BlueBlockState { get; set; }

        public int? BattaryBlock1State { get; set; }

        public int? BattaryBlock2State { get; set; }

        public int? RobotState { get; set; }

        public int? Wall1State { get; set; }

        public int? Wall2State { get; set; }

        public int? Time1 { get; set; }

        public int? Time2 { get; set; }

        public int? AdditionalTask { get; set; }

        public int Saved { get; set; }

    }
}
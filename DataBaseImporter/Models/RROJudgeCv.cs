using System.ComponentModel.DataAnnotations;

namespace DataBaseImporter.Models
{
    public class RROJudgeCv
    {
        [Key]
        public string JudgeId { get; set; }
        public int Polygon { get; set; }
        public string JudgeName { get; set; }
        public string PassHash { get; set; }
    }
}
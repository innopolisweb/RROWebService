using System.ComponentModel.DataAnnotations;

namespace DataBaseImporter.Models
{
    public class RROJudgeFin
    {
        [Key]
        public string JudgeId { get; set; }
        public string Pass { get; set; }
        public int Polygon { get; set; }
        public string JudgeName { get; set; }
    }
}
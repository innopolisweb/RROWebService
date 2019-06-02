using System.ComponentModel.DataAnnotations;

namespace DataBaseImporter.Models
{
    public class RROTeam
    {
        [Key]
        public string TeamId { get; set; }
        public int Polygon { get; set; }
        public string CategoryId { get; set; }
        public string Tour { get; set; }
        
    }
}
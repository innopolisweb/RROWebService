using System.ComponentModel.DataAnnotations;

namespace DataBaseImporter.Models
{
    public class CurrentRound
    {
        [Key]
        public int Current { get; set; }
    }
}
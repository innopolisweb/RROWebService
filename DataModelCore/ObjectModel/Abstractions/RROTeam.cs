using System.ComponentModel.DataAnnotations;

namespace DataModelCore.ObjectModel.Abstractions
{
    public class RROTeam
    {
        [Key]
        public string TeamId { get; set; }

        public int Polygon { get; set; }

        public string Category { get; set; }

    }
}
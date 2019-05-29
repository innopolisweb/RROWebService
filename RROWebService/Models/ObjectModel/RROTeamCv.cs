using System.ComponentModel.DataAnnotations;
using RROWebService.Models.ObjectModel.Abstractions;

namespace RROWebService.Models.ObjectModel
{
    public class RROTeamCv : RROTeam
    {
        /// <summary>
        /// Team identificator specifing team's category
        /// </summary>
        
        [Key]
        public string TeamId { get; set; }

        /// <summary>
        /// Index of the polygon team is binded to
        /// </summary>
        public int Polygon { get; set; }

        /// <summary>
        /// Category identifier for the current team
        /// </summary>
        
        public string Category { get; set; }
    }
}
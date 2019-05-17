using System.ComponentModel.DataAnnotations;

namespace RROWebService.Models.ObjectModel
{
    public class RROTeam
    {
        /// <summary>
        /// Team identificator specifing team's category
        /// </summary>
        
        [Key]
        public string TeamId { get; set; }

        /// <summary>
        /// Tour that current team participating
        /// </summary>
        public string Tour { get; set; }

        /// <summary>
        /// Index of the polygon team is binded to
        /// </summary>
        public int Polygon { get; set; }

        /// <summary>
        /// Category identifier for the current team
        /// </summary>
        
        public string CategoryId { get; set; }
    }
}
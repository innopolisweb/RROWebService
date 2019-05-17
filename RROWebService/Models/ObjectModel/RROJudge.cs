using System.ComponentModel.DataAnnotations;

namespace RROWebService.Models.ObjectModel
{
    public class RROJudge
    {
        /// <summary>
        /// Unique identifier for judge
        /// </summary>
        [Key]
        public string JudgeId { get; set; }

        /// <summary>
        /// Judge's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Current tour judge is participating now
        /// </summary>
        public string Tour { get; set; }

        /// <summary>
        /// Polygon associated to this judge
        /// </summary>
        public int Polygon { get; set; }

        /// <summary>
        /// Passcode for judge that uses for authorization
        /// </summary>
        public string Pass { get; set; }
    }
}
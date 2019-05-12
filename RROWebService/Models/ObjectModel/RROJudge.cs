namespace RROWebService.Models.ObjectModel
{
    public class RROJudge
    {
        /// <summary>
        /// Unique identifier for judge
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Judge's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Current tour judge is participating now
        /// </summary>
        public string Toure { get; set; }

        /// <summary>
        /// Polygon associated to this judge
        /// </summary>
        public int Polygon { get; set; }
    }
}
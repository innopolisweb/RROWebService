using System.ComponentModel.DataAnnotations;

namespace DataModelCore.ObjectModel.Primitives
{
    public class CurrentRound
    {
        [Key]
        public int Current { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace DataModelCore.ObjectModel.Primitives
{
    public class CurrentTour
    {
        [Key]
        public int Current { get; set; }
    }
}
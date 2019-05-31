using System.ComponentModel.DataAnnotations;

namespace RROWebService.Models.ObjectModel.Primitives
{
    public class CurrentTour
    {
        [Key]
        public int Current { get; set; }
    }
}
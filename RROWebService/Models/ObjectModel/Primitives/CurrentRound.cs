using System.ComponentModel.DataAnnotations;

namespace RROWebService.Models.ObjectModel.Primitives
{
    public class CurrentRound
    {
        [Key]
        public int Current { get; set; }
    }
}
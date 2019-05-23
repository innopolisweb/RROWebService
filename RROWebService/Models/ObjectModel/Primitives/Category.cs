using System.ComponentModel.DataAnnotations;

namespace RROWebService.Models.ObjectModel.Primitives
{
    public class Category
    {
        [Key]
        public string CategoryId { get; set; }

        public string FormPath { get; set; }
    }
}
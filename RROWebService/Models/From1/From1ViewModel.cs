using System.Collections.Generic;

namespace RROWebService.Models.From1
{
    public class Form1ViewModel
    {
        public Form1ViewModel()
        {
            Items = new List<Form1Item>();
        }

        public IList<Form1Item> Items { get; set; }

        public string Tour { get; set; }

        public int Polygon { get; set; }

        public string JudgeName { get; set; }

        public string Category { get; set; }
    }
}
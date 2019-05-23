using System.Collections.Generic;

namespace RROWebService.Models.Categories.Oml
{
    public class OmlViewModel
    {
        public OmlViewModel()
        {
            Teams = new List<OmlScoreViewModel>();
        }

        public List<OmlScoreViewModel> Teams { get; set; }

        public int CurrentRound { get; set; }

        public string Category { get; set; }

        public string JudgeId { get; set; }

        public string JudgeName { get; set; }

        public int Polygon { get; set; }

        public string Tour { get; set; }

    }
}
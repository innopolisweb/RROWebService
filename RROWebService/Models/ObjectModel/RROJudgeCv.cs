using System.ComponentModel.DataAnnotations;
using RROWebService.Models.ObjectModel.Abstractions;

namespace RROWebService.Models.ObjectModel
{
    public class RROJudgeCv : RROJudge
    {
        public string PassHash { get; set; }

    }
}
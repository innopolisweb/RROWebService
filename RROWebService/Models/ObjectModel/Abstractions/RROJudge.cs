﻿using System.ComponentModel.DataAnnotations;

namespace RROWebService.Models.ObjectModel.Abstractions
{
    public class RROJudge
    {
        [Key]
        public string JudgeId { get; set; }

        public string Status { get; set; }

        public int Polygon { get; set; }

        public string JudgeName { get; set; }

    }
}
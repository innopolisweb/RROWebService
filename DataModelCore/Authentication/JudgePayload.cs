using System;
using DataModelCore.ObjectModel.Abstractions;

namespace DataModelCore.Authentication
{
    public class JudgePayload
    {
        public string JudgeId { get; set; }

        public DateTime OpenTime { get; set; }

        public DateTime Expires { get; set; }

        public string Status { get; set; }

        public int Tour { get; set; }

        public string Service { get; set; }

        public static JudgePayload Create(RROJudge judge, int tour, string service)
        {
            var payload = new JudgePayload
            {
                JudgeId = judge.JudgeId,
                Status = judge.Status,
                OpenTime = DateTime.Now,
                Tour = tour,
                Service = service
            };
            if (service != "androidApp")
                payload.Expires = payload.OpenTime + TimeSpan.FromMinutes(40);
            else 
                payload.Expires = DateTime.MaxValue;
            return payload;
        }

    }
}
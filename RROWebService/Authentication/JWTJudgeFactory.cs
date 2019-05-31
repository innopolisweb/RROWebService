using System;
using System.Collections.Generic;
using System.Linq;

namespace RROWebService.Authentication
{
    public static class JWTJudgeFactory
    {
        private static readonly Dictionary<string, List<string>> Tokens 
            = new Dictionary<string, List<string>>();

        public static void AddToken(string judgeId, string token, bool removePrev = false)
        {
            if (!Tokens.ContainsKey(judgeId))
                Tokens[judgeId] = new List<string>();

            if (removePrev)
                Tokens[judgeId].Clear();

            Tokens[judgeId].Add(token);
        }

        public static string GetLastTokenForJudge(string judgeId)
        {
            if (!Tokens.ContainsKey(judgeId)) return null;

            return Tokens[judgeId].Last();
        }

        public static IEnumerable<string> GetAllTokensForJudge(string judgeId)
        {
            if (String.IsNullOrEmpty(judgeId)) return new List<string>();
            if (!Tokens.ContainsKey(judgeId)) return new List<string>();

            return Tokens[judgeId];

        }
    }
}
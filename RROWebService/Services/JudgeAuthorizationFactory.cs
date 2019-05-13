using System.Collections.Generic;
using System.Linq;

namespace RROWebService.Services
{
    public static class JudgeAuthorizationFactory
    {
        private static readonly Dictionary<string, JudgeTokenInfo> JudgeTokenInfos = 
            new Dictionary<string, JudgeTokenInfo>();

        public static int AuthorizeOrRenew(string judgeId, string pass)
        {
            var tokenInfo = JudgeTokenInfo.CreateTokenInstance(judgeId, pass);
            JudgeTokenInfos[judgeId] = tokenInfo;
            return tokenInfo.GetToken();
        }

        public static JudgeTokenValidation CheckValidation(int token)
        {
            if (JudgeTokenInfos.Count == 0) return JudgeTokenValidation.Invalid;

            if (JudgeTokenInfos.All(it => it.Value.Validate(token) == JudgeTokenValidation.Invalid))
                return JudgeTokenValidation.Invalid;

            if (JudgeTokenInfos.Any(it => it.Value.Validate(token) == JudgeTokenValidation.Expired))
                return JudgeTokenValidation.Expired;

            return JudgeTokenValidation.Valid;
        }
    }
}
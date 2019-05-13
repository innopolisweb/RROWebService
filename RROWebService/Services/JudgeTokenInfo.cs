using System;

namespace RROWebService.Services
{
    public enum JudgeTokenValidation
    {
        Invalid,
        Expired,
        Valid
    }

    public class JudgeTokenInfo
    {
        public DateTime CreationTime { get; }

        public DateTime EndingTime { get; }

        private readonly string _judgeId;
        private readonly int _passHashCode;

        private JudgeTokenInfo(string judgeId, string pass)
        {
            _judgeId = judgeId;
            _passHashCode = pass.GetHashCode();
            CreationTime = DateTime.Now;
            EndingTime = CreationTime + TimeSpan.FromSeconds(1200);
        }

        public JudgeTokenValidation Validate(int token)
        {
            var awaited = token ^ _passHashCode;
            if (awaited == (_judgeId.GetHashCode() & CreationTime.GetHashCode() & EndingTime.GetHashCode()))
            {
                return EndingTime > DateTime.Now ? JudgeTokenValidation.Valid : JudgeTokenValidation.Expired;
            }

            return JudgeTokenValidation.Invalid;
        }

        public int GetToken()
        {
            return (_judgeId.GetHashCode() & CreationTime.GetHashCode() & CreationTime.GetHashCode()) ^ _passHashCode;
        }

        public static JudgeTokenInfo CreateTokenInstance(string judgeId, string pass)
        {
            return new JudgeTokenInfo(judgeId, pass);
        }
    }
}
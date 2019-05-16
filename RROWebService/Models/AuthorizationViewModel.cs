namespace RROWebService.Models
{
    public class AuthorizationViewModel
    {
        public AuthorizationViewModel()
        {
            JudgeId = "";
            Pass = "";
            Secondary = false;
            Error = false;
        }

        public string JudgeId { get; set; }

        public string Pass { get; set; }

        public bool Secondary { get; set; }

        public bool Error { get; set; }
    }
}
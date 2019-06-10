namespace DataModelCore.Authentication
{
    public class JWTHeader
    {
        public string Algorithm { get; set; }

        public string Type { get; set; } = "JWT";
    }
}
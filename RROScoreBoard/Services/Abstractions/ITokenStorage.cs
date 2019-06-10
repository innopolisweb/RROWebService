namespace RROScoreBoard.Services.Abstractions
{
    public interface ITokenStorage
    {
        void StoreToken(string token);
        string GetToken();
    }
}
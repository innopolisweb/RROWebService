using System.IO;
using RROScoreBoard.Services.Abstractions;

namespace RROScoreBoard.Services
{
    public class InternalTokenStorage : ITokenStorage
    {
        private readonly string _dataFolder;
        public InternalTokenStorage(string dataFolder)
        {
            _dataFolder = dataFolder;
        }
        public void StoreToken(string token)
        {
            var file = Path.Combine(_dataFolder, "token");
            File.WriteAllText(file, token);
        }

        public string GetToken()
        {
            var file = Path.Combine(_dataFolder, "token");
            if (File.Exists(file))
            {
                return File.ReadAllText(file);
            }

            return null;
        }
    }
}
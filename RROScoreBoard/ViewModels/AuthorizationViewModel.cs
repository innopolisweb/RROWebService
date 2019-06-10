using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RROScoreBoard.Annotations;

namespace RROScoreBoard.ViewModels
{
    public class AuthorizationViewModel : INotifyPropertyChanged
    {
        private string _login;
        private string _pass;
        private bool _isBusy;
        private bool _error;
        private string _token;

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _pass;
            set
            {
                _pass = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public bool Error
        {
            get => _error;
            set
            {
                _error = value;
                OnPropertyChanged();
            }
        }

        public string Token
        {
            get => _token;
            set
            {
                _token = value;
                OnPropertyChanged();
            }
        }

        public async Task<string> GetToken()
        {
            IsBusy = true;
            var passBytes = Encoding.UTF8.GetBytes(Password);
            var passHashBytes = MD5.Create().ComputeHash(passBytes);
            var passHash = Convert.ToBase64String(passHashBytes);

            var response = await new HttpClient().GetAsync(
                $"https://rro.azurewebsites.net/api/authorize?judgeId={Login}&pass={passHash}&serviceId=androidApp");
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                IsBusy = false;
                Error = false;
                return token;
            }
            IsBusy = false;
            Error = true;
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
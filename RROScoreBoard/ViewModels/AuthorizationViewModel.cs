using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Android.Util;
using DataModelCore.Authentication;
using Newtonsoft.Json;
using RROScoreBoard.Annotations;

namespace RROScoreBoard.ViewModels
{
    public class AuthorizationViewModel : INotifyPropertyChanged
    {
        public delegate void AuthorizationErrorEventHandler(object sender, AuthorizationErrorEventArgs eventArgs);

        public event AuthorizationErrorEventHandler ErrorOccured;

        private string _login;
        private string _pass;
        private bool _isBusy;
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
            string token = null;
            try
            {
                var passBytes = Encoding.UTF8.GetBytes(Password);
                var passHashBytes = MD5.Create().ComputeHash(passBytes);
                var passHash = Convert.ToBase64String(passHashBytes);

                var response = await new HttpClient().GetAsync(
                    $"https://rro.azurewebsites.net/api/authorize?judgeId={Login}&pass={passHash}&serviceId=androidApp");

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var state = JsonConvert.DeserializeObject<AuthenticationError>(content);
                    switch (state)
                    {
                        case AuthenticationError.UserNotFound:
                            throw new AuthorizationException(AuthorizationException.AuthorizationErrorType
                                .UserNotFound);
                        case AuthenticationError.IncorrectPassword:
                            throw new AuthorizationException(AuthorizationException.AuthorizationErrorType
                                .InvalidPassword);
                        default: throw new Exception();
                    }
                }

                token = await response.Content.ReadAsStringAsync();
            }
            catch (AuthorizationException e)
            {
                Log.Error("RROScoreBoard", e.StackTrace);
                ErrorOccured?.Invoke(this,
                    new AuthorizationErrorEventArgs(
                        e.ErrorType == AuthorizationException.AuthorizationErrorType.UserNotFound
                            ? AuthorizationError.UserNotFound
                            : AuthorizationError.InvalidPassword));
            }
            catch (HttpRequestException e)
            {
                Log.Error("RROScoreBoard", e.StackTrace);
                ErrorOccured?.Invoke(this, new AuthorizationErrorEventArgs(AuthorizationError.NoNetwork));
            }
            catch (Exception e)
            {
                Log.Error("RROScoreBoard", e.StackTrace);
                ErrorOccured?.Invoke(this, new AuthorizationErrorEventArgs(AuthorizationError.Unknown));
            }
            finally
            {
                IsBusy = false;
            }

            return token;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public enum AuthorizationError
        {
            UserNotFound,
            InvalidPassword,
            NoNetwork,
            Unknown
        }

        public class AuthorizationErrorEventArgs : EventArgs
        {
            public AuthorizationErrorEventArgs(AuthorizationError error)
            {
                ErrorType = error;
            }
            public AuthorizationError ErrorType { get; }
        }

        private class AuthorizationException : Exception
        {
            public AuthorizationException(AuthorizationErrorType errorType)
            {
                ErrorType = errorType;
            }
            public enum AuthorizationErrorType
            {
                UserNotFound,
                InvalidPassword
            }

            public AuthorizationErrorType ErrorType { get; }
        }
    }
}
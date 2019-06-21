using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using DataModelCore.ObjectModel;
using Newtonsoft.Json;
using RROScoreBoard.Annotations;
using RROScoreBoard.Services;
using RROScoreBoard.Services.Abstractions;

namespace RROScoreBoard.ViewModels
{
    public class OmlItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ITokenStorage _tokenStorage;

        private bool _error;
        private bool _isBusy;
        private bool _isSending;
        private string _teamId;
        private int? _redBlockState;
        private int? _yellowBlockState;
        private int? _greenBlockState;
        private int? _whiteBlock1State;
        private int? _whiteBlock2State;
        private int? _blueBlockState;
        private int? _battaryBlock1State;
        private int? _battaryBlock2State;
        private int? _robotState;
        private int? _wall1State;
        private int? _wall2State;
        private int? _time1;
        private int? _time2;
        private int _saved;

        public OmlItemViewModel()
        {
            _tokenStorage = ServiceProvider.GetService<ITokenStorage>();
        }

        public OmlItemViewModel(OmlScore omlScore)
        {
            _tokenStorage = ServiceProvider.GetService<ITokenStorage>();

            _teamId = omlScore.TeamId;
            _redBlockState = omlScore.RedBlockState;
            _yellowBlockState = omlScore.YellowBlockState;
            _greenBlockState = omlScore.GreenBlockState;
            _whiteBlock1State = omlScore.WhiteBlock1State;
            _whiteBlock2State = omlScore.WhiteBlock2State;
            _blueBlockState = omlScore.BlueBlockState;
            _battaryBlock1State = omlScore.BattaryBlock1State;
            _battaryBlock2State = omlScore.BattaryBlock2State;
            _robotState = omlScore.RobotState;
            _wall1State = omlScore.Wall1State;
            _wall2State = omlScore.Wall2State;
            _time1 = omlScore.Time1;
            _time2 = omlScore.Time2;
            _saved = omlScore.Saved;
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

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public bool IsSending
        {
            get => _isSending;
            set
            {
                _isSending = value;
                OnPropertyChanged();
            }
        }

        public string TeamId
        {
            get => _teamId;
            set
            {
                _teamId = value;
                OnPropertyChanged();
            }
        }

        public int? RedBlockState
        {
            get => _redBlockState;
            set
            {
                _redBlockState = value;
                OnPropertyChanged();
            }
        }

        public int? YellowBlockState
        {
            get => _yellowBlockState;
            set
            {
                _yellowBlockState = value;
                OnPropertyChanged();
            }
        }

        public int? GreenBlockState
        {
            get => _greenBlockState;
            set
            {
                _greenBlockState = value;
                OnPropertyChanged();
            }
        }

        public int? WhiteBlock1State
        {
            get => _whiteBlock1State;
            set
            {
                _whiteBlock1State = value;
                OnPropertyChanged();
            }
        }

        public int? WhiteBlock2State
        {
            get => _whiteBlock2State;
            set
            {
                _whiteBlock2State = value;
                OnPropertyChanged();
            }
        }

        public int? BlueBlockState
        {
            get => _blueBlockState;
            set
            {
                _blueBlockState = value;
                OnPropertyChanged(
);
            }
        }

        public int? BattaryBlock1State
        {
            get => _battaryBlock1State;
            set
            {
                _battaryBlock1State = value;
                OnPropertyChanged(
);
            }
        }

        public int? BattaryBlock2State
        {
            get => _battaryBlock2State;
            set
            {
                _battaryBlock2State = value;
                OnPropertyChanged(
);
            }
        }

        public int? RobotState
        {
            get => _robotState;
            set
            {
                _robotState = value;
                OnPropertyChanged();
            }
        }

        public int? Wall1State
        {
            get => _wall1State;
            set
            {
                _wall1State = value;
                OnPropertyChanged(
);
            }
        }

        public int? Wall2State
        {
            get => _wall2State;
            set
            {
                _wall2State = value;
                OnPropertyChanged(
);
            }
        }

        public int? Time1
        {
            get => _time1;
            set
            {
                _time1 = value;
                OnPropertyChanged();
            }
        }

        public int? Time2
        {
            get => _time2;
            set
            {
                _time2 = value;
                OnPropertyChanged();
            }
        }

        public int Saved
        {
            get => _saved;
            set
            {
                _saved = value;
                OnPropertyChanged();
            }
        }

        public void ClearEventListeners()
        {
            PropertyChanged = null;
        }

        public void BeginInitialization()
        {
            IsBusy = true;
        }

        public void EndInitialization()
        {
            IsBusy = false;
        }

        public async void Send(bool saved = false)
        {
            if (saved) IsSending = true;

            var token = _tokenStorage.GetToken();
            var vm = ServiceProvider.GetService<ScoreBoardViewModel>();
            var score = new OmlScore
            {
                TeamId = TeamId,
                JudgeId = vm.JudgeId,
                Round = vm.Round.Value,
                Polygon = vm.Polygon.Value,
                RedBlockState = RedBlockState,
                YellowBlockState = YellowBlockState,
                GreenBlockState = GreenBlockState,
                WhiteBlock1State = WhiteBlock1State,
                WhiteBlock2State = WhiteBlock2State,
                BlueBlockState = BlueBlockState,
                BattaryBlock1State = BattaryBlock1State,
                BattaryBlock2State = BattaryBlock2State,
                RobotState = RobotState,
                Wall1State = Wall1State,
                Wall2State = Wall2State,
                Time1 = Time1,
                Time2 = Time2,
                Saved = saved ? 1 : 0
            };
            var client = new HttpClient();
            var jsonString = JsonConvert.SerializeObject(score);
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://rro.azurewebsites.net/api/omlpreresult"),
                Content = new StringContent(jsonString, Encoding.UTF8, "application/json"),
                Headers =
                {
                    {"token", token}
                },
                Method = HttpMethod.Post
            };
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                if (saved) Error = true;
                IsSending = false;
                return;
            }

            var newToken = await response.Content.ReadAsStringAsync();
            _tokenStorage.StoreToken(newToken);
            Saved = 1;
            IsSending = false;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using DataModelCore.ObjectModel.Abstractions;
using Newtonsoft.Json;
using RROScoreBoard.Annotations;
using RROScoreBoard.Services;
using RROScoreBoard.Services.Abstractions;

namespace RROScoreBoard.ViewModels
{
    public class ScoreBoardViewModel : INotifyPropertyChanged
    {
        private readonly ITokenStorage _tokenStorage;

        private string _category;
        private bool _isBusy;
        private bool _error;
        private int _tour;
        private string _judgeName;
        private int _round;
        private int _polygon;

        public ScoreBoardViewModel()
        {
            _tokenStorage = ServiceProvider.GetService<ITokenStorage>();
        }

        public string Category
        {
            get => _category;
            set
            {
                _category = value;
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

        public int Tour
        {
            get => _tour;
            set
            {
                _tour = value;
                OnPropertyChanged();
            }
        }

        public string JudgeName
        {
            get => _judgeName;
            set
            {
                _judgeName = value;
                OnPropertyChanged();
            }
        }

        public int Round
        {
            get => _round;
            set
            {
                _round = value;
                OnPropertyChanged();
            }
        }

        public int Polygon
        {
            get => _polygon;
            set
            {
                _polygon = value;
                OnPropertyChanged();
            }
        }

        public async void LoadData()
        {
            IsBusy = true;
            var token = _tokenStorage.GetToken();
            var judgeResponse = await new HttpClient().GetAsync($"https://rro.azurewebsites.net/api/judge?token={token}");
            if (!judgeResponse.IsSuccessStatusCode)
            {
                Error = true;
                IsBusy = false;
                return;
            }
                                                 
            var judge = JsonConvert.DeserializeObject<RROJudge>(await judgeResponse.Content.ReadAsStringAsync());
            JudgeName = judge.JudgeName;
            Polygon = judge.Polygon;

            var tourResponse = await new HttpClient().GetAsync("https://rro.azurewebsites.net/api/currenttour");
            if (!tourResponse.IsSuccessStatusCode)
            {
                Error = true;
                IsBusy = false;
                return;
            }

            var tour = Int32.Parse(await tourResponse.Content.ReadAsStringAsync());
            Tour = tour;

            var teamsResponse =
                await new HttpClient().GetAsync(
                    $"https://rro.azurewebsites.net/api/teams?tour={Tour}&polygon={Polygon}");
            if (!teamsResponse.IsSuccessStatusCode)
            {
                Error = true;
                IsBusy = false;
                return;
            }

            var teams = JsonConvert.DeserializeObject<List<RROTeam>>(await teamsResponse.Content.ReadAsStringAsync());
            if (!teams.Any())
            {
                Error = true;
                IsBusy = false;
                return;
            }

            var category = teams.First().Category;
            Category = category;

            var roundResponse = await new HttpClient().GetAsync("https://rro.azurewebsites.net/api/currentround");
            if (!roundResponse.IsSuccessStatusCode)
            {
                Error = true;
                IsBusy = false;
                return;
            }

            var round = Int32.Parse(await roundResponse.Content.ReadAsStringAsync());
            Round = round;

            IsBusy = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
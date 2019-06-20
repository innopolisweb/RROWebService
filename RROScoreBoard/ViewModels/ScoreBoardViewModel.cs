using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Android.Util;
using DataModelCore.ObjectModel.Abstractions;
using Newtonsoft.Json;
using RROScoreBoard.Annotations;
using RROScoreBoard.Services;
using RROScoreBoard.Services.Abstractions;

namespace RROScoreBoard.ViewModels
{
    public class ScoreBoardViewModel : INotifyPropertyChanged
    {

        public delegate void ScoreBoardErrorEventHandler(object sender, ScoreBoardErrorEventArgs eventArgs);

        public event ScoreBoardErrorEventHandler ErrorOccured;

        private readonly ITokenStorage _tokenStorage;

        private string _category;
        private string _judgeId;
        private bool _isBusy;
        private int? _tour;
        private string _judgeName;
        private int? _round;
        private int? _polygon;
        private List<RROTeam> _teams;

        public ScoreBoardViewModel()
        {
            _tokenStorage = ServiceProvider.GetService<ITokenStorage>();
        }

        public string JudgeId
        {
            get => _judgeId;
            set
            {
                _judgeId = value;
                OnPropertyChanged();
            }
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

        public int? Tour
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

        public int? Round
        {
            get => _round;
            set
            {
                _round = value;
                OnPropertyChanged();
            }
        }

        public int? Polygon
        {
            get => _polygon;
            set
            {
                _polygon = value;
                OnPropertyChanged();
            }
        }

        public List<RROTeam> Teams
        {
            get => _teams;
            set
            {
                _teams = value;
                OnPropertyChanged();
            }
        }

        public async void LoadData(bool forceNet = false)
        {
            if (Teams != null && !forceNet) return;

            IsBusy = true;
            try
            {
                var token = _tokenStorage.GetToken();

                var judgeResponse =
                    await new HttpClient().GetAsync($"https://rro.azurewebsites.net/api/judge?token={token}");
                var judge = JsonConvert.DeserializeObject<RROJudge>(await judgeResponse.Content.ReadAsStringAsync());

                var tourResponse = await new HttpClient().GetAsync("https://rro.azurewebsites.net/api/currenttour");
                var tour = Int32.Parse(await tourResponse.Content.ReadAsStringAsync());

                if (tour == -1) throw new ScoreBoardException(ScoreBoardError.TourNotStarted);


                var teamsResponse = await new HttpClient().GetAsync(
                    $"https://rro.azurewebsites.net/api/teams?tour={tour}&polygon={judge.Polygon}");
                var teams = JsonConvert.DeserializeObject<List<RROTeam>>(
                    await teamsResponse.Content.ReadAsStringAsync());

                var category = teams.First().Category;

                var roundResponse =
                    await new HttpClient().GetAsync(
                        $"https://rro.azurewebsites.net/api/currentround?category={category}");
                var round = Int32.Parse(await roundResponse.Content.ReadAsStringAsync());

                if (round == -1) throw new ScoreBoardException(ScoreBoardError.RoundNotStarted);


                Category = category;
                Tour = tour;
                JudgeName = judge.JudgeName;
                Polygon = judge.Polygon;
                JudgeId = judge.JudgeId;
                Round = round;
                Teams = teams;
            }
            catch (HttpRequestException e)
            {
                Log.Error("RROScoreBoard", e.StackTrace);
                ErrorOccured?.Invoke(this, new ScoreBoardErrorEventArgs(ScoreBoardError.NoInternet));
            }
            catch (ScoreBoardException e)
            {
                Log.Error("RROScoreBoard", e.StackTrace);
                ErrorOccured?.Invoke(this, new ScoreBoardErrorEventArgs(e.ErrorType));

            }
            catch (Exception e)
            {
                Log.Error("RROScoreBoard", e.StackTrace);
                ErrorOccured?.Invoke(this, new ScoreBoardErrorEventArgs(ScoreBoardError.Unknown));
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public enum ScoreBoardError
        {
            TourNotStarted,
            RoundNotStarted,
            NoInternet,
            Unknown
        }

        public class ScoreBoardErrorEventArgs : EventArgs
        {
            public ScoreBoardErrorEventArgs(ScoreBoardError errorType)
            {
                ErrorType = errorType;
            }
            public ScoreBoardError ErrorType { get; }
        }

        private class ScoreBoardException : Exception
        {
            public ScoreBoardException(ScoreBoardError errorType)
            {
                ErrorType = errorType;
            }
            public ScoreBoardError ErrorType { get; }
        }
    }
}
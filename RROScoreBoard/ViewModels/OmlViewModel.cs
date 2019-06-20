using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using DataModelCore.ObjectModel;
using DataModelCore.ObjectModel.Abstractions;
using Newtonsoft.Json;
using RROScoreBoard.Annotations;

namespace RROScoreBoard.ViewModels
{
    public class OmlViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<OmlItemViewModel> _teams;

        public ObservableCollection<OmlItemViewModel> Teams
        {
            get => _teams;
            private set
            {
                _teams = value;
                OnPropertyChanged();
            }
        }


        public async void LoadData(List<RROTeam> teams, bool forceNet = false)
        {
            if (Teams != null && !forceNet) return;

            if (!teams.Any()) throw new Exception("List of teams cannot be empty");

            Teams = new ObservableCollection<OmlItemViewModel>(from team in teams select new OmlItemViewModel { TeamId = team.TeamId });

            foreach (var team in Teams) team.BeginInitialization();

            var tmpTeam = teams.First();

            var preResultsRequest = await new HttpClient().GetAsync($"https://rro.azurewebsites.net/api/omlpreresults?polygon={tmpTeam.Polygon}");
            if (!preResultsRequest.IsSuccessStatusCode)
            {
                foreach (var team in Teams)
                {
                    team.Error = true;
                    team.EndInitialization();
                }
                return;
            }

            var preResultsJson = await preResultsRequest.Content.ReadAsStringAsync();
            var preResults = JsonConvert.DeserializeObject<List<OmlScore>>(preResultsJson);

            foreach (var preResult in preResults)
            {
                var teamVm = Teams.First(team => team.TeamId == preResult.TeamId);
                teamVm.RedBlockState = preResult.RedBlockState;
                teamVm.YellowBlockState = preResult.YellowBlockState;
                teamVm.GreenBlockState = preResult.GreenBlockState;
                teamVm.WhiteBlock1State = preResult.WhiteBlock1State;
                teamVm.WhiteBlock2State = preResult.WhiteBlock2State;
                teamVm.BlueBlockState = preResult.BlueBlockState;
                teamVm.BattaryBlock1State = preResult.BattaryBlock1State;
                teamVm.BattaryBlock2State = preResult.BattaryBlock2State;
                teamVm.RobotState = preResult.RobotState;
                teamVm.Wall1State = preResult.Wall1State;
                teamVm.Wall2State = preResult.Wall2State;
                teamVm.Time1 = preResult.Time1;
                teamVm.Time2 = preResult.Time2;
                teamVm.Saved = preResult.Saved;

            }

            foreach (var team in Teams) team.EndInitialization();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using DataBaseImporter.Annotations;
using DataBaseImporter.Helpers;
using DataBaseImporter.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace DataBaseImporter.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _sheetId;
        private SheetsService _service;
        private bool _readyForDb;
        private bool _isLoading;
        private bool _isSending;
        private bool _isLoadingError;
        private bool _isSendingError;
        private bool _sent;

        public MainViewModel()
        {

            EnterSheetIdCommand = new Command(GetSheetId, o => !IsLoading);
            InitializeServiceCommand = new AsyncCommand(Initialize);
            RecieveDataCommand = new AsyncCommand(RecieveData);
            FillDbCommand = new AsyncCommand(FillDb);

            Teams = new ObservableCollection<RROTeam>();
            JudgesCv = new ObservableCollection<RROJudgeCv>();
            JudgesFin = new ObservableCollection<RROJudgeFin>();
        }

        public Command EnterSheetIdCommand { get; }
        
        public AsyncCommand FillDbCommand { get; }

        public AsyncCommand InitializeServiceCommand { get; }

        public AsyncCommand RecieveDataCommand { get; }

        public ObservableCollection<RROTeam> Teams { get; }

        public ObservableCollection<RROJudgeCv> JudgesCv { get; }

        public ObservableCollection<RROJudgeFin> JudgesFin { get; }

        public bool ReadyForDb
        {
            get => _readyForDb;
            private set
            {
                _readyForDb = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public bool IsSending
        {
            get => _isSending;
            private set
            {
                _isSending = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoadingError
        {
            get => _isLoadingError;
            private set
            {
                _isLoadingError = value;
                OnPropertyChanged();
            }
        }

        public bool IsSendingError
        {
            get => _isSendingError;
            private set
            {
                _isSendingError = value;
                OnPropertyChanged();
            }
        }

        public bool Sent
        {
            get => _sent;
            set
            {
                _sent = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void GetSheetId(object obj)
        {
            Sent = false;

            var dialog = new SheetIdWindow {Owner = (Window) obj};
            dialog.ShowDialog();
            _sheetId = dialog.SheetId;

            if (String.IsNullOrWhiteSpace(_sheetId))
            {
                return;
            }
            
            try
            {
                _service.Spreadsheets.Values.Get(_sheetId, "Teams!A:A").Execute();
            }
            catch (Exception)
            {
                IsLoadingError = true;
                return;
            }

            RecieveDataCommand.Execute(null);
        }

        private void Initialize(object obj)
        {
            Sent = false;
            IsLoading = true;
            var cred = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.ClientId)))
                    .Secrets,
                new[] { SheetsService.Scope.Spreadsheets }, "dbinitializer", CancellationToken.None).Result;

            _service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred,
                ApplicationName = "DATABASEInitializer"
            });
            IsLoading = false;
        }

        private void RecieveData(object obj)
        {
            IsLoading = true;

            RecieveDataCommand.ReportProgress(() =>
            {
                Teams.Clear();
                JudgesCv.Clear();
                JudgesFin.Clear();

            });

            var teamIds = _service.Spreadsheets.Values.Get(_sheetId, "Teams!A:A").Execute();
            var polygons = _service.Spreadsheets.Values.Get(_sheetId, "Teams!B:B").Execute();
            for (var i = 0; i < teamIds.Values.Count; ++i)
            {
                if (teamIds.Values[i][0].ToString().Trim().ToLower() == "teamid") continue;

                var data = teamIds.Values[i][0].ToString().Trim().Split('-');
                var i1 = i;
                InitializeServiceCommand.ReportProgress(() =>
                {
                    var parsed = Int32.TryParse(polygons.Values[i1][0].ToString().Trim(), out var res);
                    Teams.Add(new RROTeam
                    {
                        TeamId = teamIds.Values[i1][0].ToString().Trim(),
                        Polygon = parsed ? res : 0,
                        Category = data[0],
                    });
                });
            }

            var judgeCvIds = _service.Spreadsheets.Values.Get(_sheetId, "JudgesCv!A:A").Execute();
            var judgeCvPass = _service.Spreadsheets.Values.Get(_sheetId, "JudgesCv!B:B").Execute();
            var judgeCvPolygons = _service.Spreadsheets.Values.Get(_sheetId, "JudgesCv!C:C").Execute();
            var judgeCvNames = _service.Spreadsheets.Values.Get(_sheetId, "JudgesCv!D:D").Execute();
            for (var i = 0; i < judgeCvIds.Values.Count; ++i)
            {
                if (judgeCvIds.Values[i][0].ToString().ToLower() == "judgeid") continue;

                var i1 = i;
                RecieveDataCommand.ReportProgress(() =>
                {
                    var parsed = Int32.TryParse(judgeCvPolygons.Values[i1][0].ToString().Trim(), out var res);
                    var pass = judgeCvPass.Values[i1][0].ToString();
                    var passBytes = Encoding.UTF8.GetBytes(pass);
                    var hash = MD5.Create().ComputeHash(passBytes);
                    var passHash = Convert.ToBase64String(hash);
                    JudgesCv.Add(new RROJudgeCv
                    {
                        JudgeId = judgeCvIds.Values[i1][0].ToString().Trim(),
                        Polygon = parsed ? res : 0,
                        PassHash = passHash,
                        JudgeName = judgeCvNames.Values[i1][0].ToString().Trim(),
                    });
                });
            }

            var judgeFinIds = _service.Spreadsheets.Values.Get(_sheetId, "JudgesFin!A:A").Execute();
            var judgeFinPass = _service.Spreadsheets.Values.Get(_sheetId, "JudgesFin!B:B").Execute();
            var judgeFinPolygons = _service.Spreadsheets.Values.Get(_sheetId, "JudgesFin!C:C").Execute();
            var judgeFinNames = _service.Spreadsheets.Values.Get(_sheetId, "JudgesFin!D:D").Execute();
            for (var i = 0; i < judgeCvIds.Values.Count; ++i)
            {
                if (judgeFinIds.Values[i][0].ToString().ToLower() == "judgeid") continue;

                var i1 = i;
                var pass = judgeFinPass.Values[i1][0].ToString();
                var passBytes = Encoding.UTF8.GetBytes(pass);
                var hash = MD5.Create().ComputeHash(passBytes);
                var passHash = Convert.ToBase64String(hash);
                RecieveDataCommand.ReportProgress(() =>
                {
                    var parsed = Int32.TryParse(judgeFinPolygons.Values[i1][0].ToString().Trim(), out var res);
                    JudgesFin.Add(new RROJudgeFin
                    {
                        JudgeId = judgeFinIds.Values[i1][0].ToString().Trim(),
                        Polygon = parsed ? res : 0,
                        PassHash = passHash,
                        JudgeName = judgeFinNames.Values[i1][0].ToString().Trim(),
                    });
                });
            }

            ReadyForDb = true;
            IsLoading = false;
        }

        private void FillDb(object obj)
        {
            ReadyForDb = false;

            var res = false;
            FillDbCommand.ReportProgress(() =>
            {
                var confirmation = new RefillBaseConfirmationWindow
                {
                    Owner = (Window) obj,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                confirmation.ShowDialog();
                res = confirmation.Success;
            });

            if (!res)
            {
                ReadyForDb = true;
                return;
            }

            IsSending = true;
            try
            {
                var context = new MainContext();
                context.TeamsCv.RemoveRange(context.TeamsCv.ToList());
                context.JudgesCv.RemoveRange(context.JudgesCv.ToList());
                context.JudgesFin.RemoveRange(context.JudgesFin.ToList());
                context.SaveChanges();

                context.TeamsCv.AddRange(Teams);
                context.JudgesCv.AddRange(JudgesCv);
                context.JudgesFin.AddRange(JudgesFin);
                context.SaveChanges();
            }
            catch (Exception)
            {
                IsSendingError = true;
                IsSending = false;
                ReadyForDb = true;
                return;
            }

            IsSending = false;
            ReadyForDb = true;
            Sent = true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
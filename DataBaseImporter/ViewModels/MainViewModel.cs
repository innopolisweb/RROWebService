﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public MainViewModel()
        {

            EnterSheetIdCommand = new Command(GetSheetId);
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
            set
            {
                _readyForDb = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void GetSheetId(object obj)
        {
            var dialog = new SheetIdWindow {Owner = (Window) obj};
            dialog.ShowDialog();
            _sheetId = dialog.SheetId;
            
            try
            {
                _service.Spreadsheets.Values.Get(_sheetId, "Teams!A:A").Execute();
            }
            catch (Exception)
            {
                MessageBox.Show("Таблицы с заданным ID не существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            RecieveDataCommand.Execute(null);
        }

        private void Initialize(object obj)
        {
            var cred = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.ClientId)))
                    .Secrets,
                new[] { SheetsService.Scope.Spreadsheets }, "dbinitializer", CancellationToken.None).Result;

            _service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred,
                ApplicationName = "DATABASEInitializer"
            });
        }

        private void RecieveData(object obj)
        {
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
                Teams.Add(new RROTeam
                {
                    TeamId = teamIds.Values[i1][0].ToString().Trim(),
                    Polygon = Int32.Parse(polygons.Values[i1][0].ToString().Trim()),
                    CategoryId = data[0],
                    Tour = "Квалификационный"
                }));
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
                JudgesCv.Add(new RROJudgeCv
                {
                    JudgeId = judgeCvIds.Values[i1][0].ToString().Trim(),
                    Polygon = Int32.Parse(judgeCvPolygons.Values[i1][0].ToString().Trim()),
                    Pass = judgeCvPass.Values[i1][0].ToString(),
                    JudgeName = judgeCvNames.Values[i1][0].ToString().Trim(),
                }));
            }

            var judgeFinIds = _service.Spreadsheets.Values.Get(_sheetId, "JudgesFin!A:A").Execute();
            var judgeFinPass = _service.Spreadsheets.Values.Get(_sheetId, "JudgesFin!B:B").Execute();
            var judgeFinPolygons = _service.Spreadsheets.Values.Get(_sheetId, "JudgesFin!C:C").Execute();
            var judgeFinNames = _service.Spreadsheets.Values.Get(_sheetId, "JudgesFin!D:D").Execute();
            for (var i = 0; i < judgeCvIds.Values.Count; ++i)
            {
                if (judgeFinIds.Values[i][0].ToString().ToLower() == "judgeid") continue;

                var i1 = i;
                RecieveDataCommand.ReportProgress(() =>
                JudgesFin.Add(new RROJudgeFin
                {
                    JudgeId = judgeFinIds.Values[i1][0].ToString().Trim(),
                    Polygon = Int32.Parse(judgeFinPolygons.Values[i1][0].ToString().Trim()),
                    Pass = judgeFinPass.Values[i1][0].ToString(),
                    JudgeName = judgeFinNames.Values[i1][0].ToString().Trim(),
                }));
            }

            ReadyForDb = true;

        }

        private void FillDb(object obj)
        {
            ReadyForDb = false;

            var context = new MainContext();
            bool res = false;
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

            context.Teams.RemoveRange(context.Teams.ToList());
            context.JudgesCv.RemoveRange(context.JudgesCv.ToList());
            context.JudgesFin.RemoveRange(context.JudgesFin.ToList());
            context.SaveChanges();

            context.Teams.AddRange(Teams);
            context.JudgesCv.AddRange(JudgesCv);
            context.JudgesFin.AddRange(JudgesFin);
            context.SaveChanges();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
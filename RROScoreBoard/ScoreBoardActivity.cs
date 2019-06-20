using System;
using System.ComponentModel;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using RROScoreBoard.Fragments;
using RROScoreBoard.Services;
using RROScoreBoard.ViewModels;

namespace RROScoreBoard
{
    [Activity(Label = "Score board")]
    public class ScoreBoardActivity : AppCompatActivity
    {
        private ScoreBoardViewModel _vm;
        private LinearLayout _loadingLayout;
        private LinearLayout _errorLayout;
        private TextView _categoryTextView;
        private TextView _roundTextView;
        private TextView _tourTextView;
        private TextView _polygonTextView;
        private TextView _errorTextView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_scoreboard);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            _categoryTextView = FindViewById<TextView>(Resource.Id.sb_category_text_view);
            _roundTextView = FindViewById<TextView>(Resource.Id.sb_round_text_view);
            _tourTextView = FindViewById<TextView>(Resource.Id.sb_tour_text_view);
            _polygonTextView = FindViewById<TextView>(Resource.Id.sb_polygon_text_view);
            _errorTextView = FindViewById<TextView>(Resource.Id.sb_error_text_view);
            var tryAgainButton = FindViewById<Button>(Resource.Id.sb_try_again_button);
            tryAgainButton.Click += TryAgainButtonOnClick;

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            _loadingLayout = FindViewById<LinearLayout>(Resource.Id.sb_loading);
            _errorLayout = FindViewById<LinearLayout>(Resource.Id.sb_error_layout);
            
            _vm = ServiceProvider.GetService<ScoreBoardViewModel>();
            _vm.PropertyChanged += VmOnPropertyChanged;
            _vm.ErrorOccured += VmOnErrorOccured;

            _vm.LoadData();
            SetViewModelValues();
        }

        private void VmOnErrorOccured(object sender, ScoreBoardViewModel.ScoreBoardErrorEventArgs eventArgs)
        {
            _errorLayout.Visibility = ViewStates.Visible;
            switch (eventArgs.ErrorType)
            {
                case ScoreBoardViewModel.ScoreBoardError.TourNotStarted:
                    _errorTextView.SetText(Resource.String.tour_not_started);
                    break;
                case ScoreBoardViewModel.ScoreBoardError.RoundNotStarted:
                    _errorTextView.SetText(Resource.String.round_not_started);
                    break;
                case ScoreBoardViewModel.ScoreBoardError.NoInternet:
                    _errorTextView.SetText(Resource.String.internet_error);
                    break;
                case ScoreBoardViewModel.ScoreBoardError.Unknown:
                    _errorTextView.SetText(Resource.String.unknown_error);
                    break;
            }

        }

        private void TryAgainButtonOnClick(object sender, EventArgs eventArgs)
        {
            _vm.LoadData(true);
        }

        protected override void OnDestroy()
        {
            _vm.PropertyChanged -= VmOnPropertyChanged;
            _vm.ErrorOccured -= VmOnErrorOccured;
            base.OnDestroy();
        }

        private void VmOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case nameof(_vm.IsBusy):
                    VmOnBusyChanged();
                    break;
                default:
                    VmOnDataChanged();
                    break;
            }
        }

        private void VmOnDataChanged()
        {
            _roundTextView.Text =
                Resources.GetString(Resource.String.round_format, _vm.IsBusy || _vm.Round == null ? "???" : _vm.Round.ToString());

            _polygonTextView.Text = Resources.GetString(Resource.String.polygon_format,
                _vm.IsBusy || _vm.Polygon == null ? "???" : _vm.Polygon.ToString());

            if (_vm.IsBusy || _vm.Tour == null)
                _tourTextView.Text = Resources.GetString(Resource.String.tour_format, "???");
            else
                _tourTextView.Text = Resources.GetString(Resource.String.tour_format,
                    _vm.Tour == 0
                        ? Resources.GetString(Resource.String.tour_cvalification)
                        : Resources.GetString(Resource.String.tour_final));

            VmOnCategryChanged();
        }

        private void VmOnCategryChanged()
        {
            if (_vm.IsBusy)
            {
                SetUnknownCategory();
                return;
            }

            if (_vm.Category == "ОМЛ") SetOmlFragment();
            else SetUnknownCategory();
        }

        private void SetUnknownCategory()
        {
            _categoryTextView.Text = Resources.GetString(Resource.String.category_format, "???");
        }

        private void VmOnBusyChanged()
        {
            if (_vm.IsBusy)
            {
                _errorLayout.Visibility = ViewStates.Gone;
                _loadingLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                _loadingLayout.Visibility = ViewStates.Gone;
            }
            VmOnDataChanged();
        }

        private void SetOmlFragment()
        {
            _categoryTextView.Text = Resources.GetString(Resource.String.category_format,
                Resources.GetString(Resource.String.category_oml));

            if (SupportFragmentManager.FindFragmentByTag(OmlScoreBoardFragment.FragmentTag) != null)
                return;

            var omlFragment = new OmlScoreBoardFragment();
            SupportFragmentManager.BeginTransaction()
                .Add(Resource.Id.sb_fragment_container, omlFragment, OmlScoreBoardFragment.FragmentTag)
                .Commit();
        }

        private void SetViewModelValues()
        {
            VmOnDataChanged();
        }
    }
}
using System;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using RROScoreBoard.Services;
using RROScoreBoard.Services.Abstractions;
using RROScoreBoard.ViewModels;

namespace RROScoreBoard
{
    [Activity(Label = "RRO scoreboard", NoHistory = true)]
    public class AuthorizationActivity : AppCompatActivity
    {
        private TextInputEditText _loginEditText;
        private TextInputEditText _passEditText;
        private LinearLayout _authForm;
        private LinearLayout _loadingLayout;
        private TextView _errorTextView;
        private Button _loginButton;
        private AuthorizationViewModel _vm;
        private ITokenStorage _tokenStorage;

        private void VmOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case nameof(_vm.IsBusy):
                    VmOnBusyChanged();
                    break;
            }
        }

        private void VmOnBusyChanged()
        {
            if (_vm.IsBusy)
            {
                _authForm.Visibility = ViewStates.Gone;
                _loadingLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                _authForm.Visibility = ViewStates.Visible;
                _loadingLayout.Visibility = ViewStates.Gone;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _vm = ServiceProvider.GetService<AuthorizationViewModel>();
            _vm.PropertyChanged += VmOnPropertyChanged;
            _vm.ErrorOccured += VmOnErrorOccured;

            _tokenStorage = ServiceProvider.GetService<ITokenStorage>();

            SetContentView(Resource.Layout.activity_authorization);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            _loginButton = FindViewById<Button>(Resource.Id.auth_login_button);
            _loginButton.Click += LoginButtonOnClick;

            _loginEditText = FindViewById<TextInputEditText>(Resource.Id.auth_login_edit_text);
            _loginEditText.TextChanged += AuthDataOnTextChanged;

            _passEditText = FindViewById<TextInputEditText>(Resource.Id.auth_pass_edit_text);
            _passEditText.TextChanged += AuthDataOnTextChanged;

            _authForm = FindViewById<LinearLayout>(Resource.Id.auth_form);
            _loadingLayout = FindViewById<LinearLayout>(Resource.Id.auth_loading);
            _errorTextView = FindViewById<TextView>(Resource.Id.auth_error_text);
        }

        private void VmOnErrorOccured(object sender, AuthorizationViewModel.AuthorizationErrorEventArgs eventArgs)
        {
            switch (eventArgs.ErrorType)
            {
                case AuthorizationViewModel.AuthorizationError.UserNotFound:
                case AuthorizationViewModel.AuthorizationError.InvalidPassword:
                    _passEditText.Text = "";
                    _errorTextView.Visibility = ViewStates.Visible;
                    break;
                case AuthorizationViewModel.AuthorizationError.NoNetwork:
                    Toast.MakeText(this, Resource.String.internet_error, ToastLength.Short).Show();
                    break;
                case AuthorizationViewModel.AuthorizationError.Unknown:
                    Toast.MakeText(this, Resource.String.unknown_error, ToastLength.Short).Show();
                    break;
            }
        }

        protected override void OnDestroy()
        {
            _vm.PropertyChanged -= VmOnPropertyChanged;
            _vm.ErrorOccured -= VmOnErrorOccured;
            base.OnDestroy();
        }

        private void AuthDataOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            _errorTextView.Visibility = ViewStates.Gone;
            if (!String.IsNullOrWhiteSpace(_loginEditText.Text) && !String.IsNullOrEmpty(_passEditText.Text))
            {
                _loginButton.Enabled = true;
            }
            else
            {
                _loginButton.Enabled = false;
            }
        }

        private async void LoginButtonOnClick(object sender, EventArgs eventArgs)
        {
            _vm.Login = _loginEditText.Text;
            _vm.Password = _passEditText.Text;
            var token = await _vm.GetToken();

            if (String.IsNullOrWhiteSpace(token)) return;

            _tokenStorage.StoreToken(token);
            var intent = new Intent(ApplicationContext, typeof(ScoreBoardActivity));
            StartActivity(intent);
            Finish();
        }
    }
}
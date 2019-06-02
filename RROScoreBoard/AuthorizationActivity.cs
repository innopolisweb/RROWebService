using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace RROScoreBoard
{
    [Activity(Label = "Authorization", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class AuthorizationActivity : AppCompatActivity
    {
        private TextInputEditText _loginEditText;
        private TextInputEditText _passEditText;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_authorization);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            var loginButton = FindViewById<Button>(Resource.Id.auth_login_button);
            loginButton.Click += LoginButtonOnClick;
        }

        private void LoginButtonOnClick(object sender, EventArgs eventArgs)
        {
            
        }
    }
}
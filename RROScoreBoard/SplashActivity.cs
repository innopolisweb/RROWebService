using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using RROScoreBoard.Services;
using RROScoreBoard.Services.Abstractions;
using RROScoreBoard.ViewModels;

namespace RROScoreBoard
{
    [Activity(NoHistory = true, Theme = "@style/MyTheme.Splash", MainLauncher = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            RegisterServices();

            var intent = new Intent(Application.Context, typeof(AuthorizationActivity));
            StartActivity(intent);
            Finish();
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Log.Error("ебанина", unhandledExceptionEventArgs.ExceptionObject.ToString());
        }

        private void RegisterServices()
        {
            ServiceProvider.AddService<ITokenStorage>(new InternalTokenStorage(ApplicationInfo.DataDir));
            ServiceProvider.AddService(new AuthorizationViewModel());
            ServiceProvider.AddService(new ScoreBoardViewModel());
            ServiceProvider.AddService(new OmlViewModel());
        }
    }
}
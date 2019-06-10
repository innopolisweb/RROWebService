using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using DataModelCore.DataContexts;
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
            RegisterServices();

            var intent = new Intent(Application.Context, typeof(AuthorizationActivity));
            StartActivity(intent);
            Finish();
        }
        private void RegisterServices()
        {
            ServiceProvider.AddService<ITokenStorage>(new InternalTokenStorage(ApplicationContext.DataDir.AbsolutePath));
            ServiceProvider.AddService(new AuthorizationViewModel());
        }
    }
}
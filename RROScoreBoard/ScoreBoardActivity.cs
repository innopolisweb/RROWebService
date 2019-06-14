using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace RROScoreBoard
{
    [Activity(Label = "Score board")]
    public class ScoreBoardActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_scoreboard);
        }
    }
}
using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using RROScoreBoard.ViewModels;
using DialogFragment = Android.Support.V4.App.DialogFragment;

namespace RROScoreBoard.Fragments
{
    public class OmlSendingConfirmationFragment : DialogFragment
    {
        public const string FragmentTag = "RROScoreBoard.fragments.OMLSENDINGCONFIRMATIONFRAGMENT";
        public delegate void ConfirmedEventHandler(OmlItemViewModel item);

        private const string ConfirmedTag = "RROScoreBoard.fragments.OMLSENDINGCONFIRMATIONFRAGMENT.Confirmed";
        private OmlItemViewModel _vm;
        private bool _confirmed;

        private TextView _teamTextView;
        private TextView _redTextView;
        private TextView _yellowTextView;
        private TextView _greenTextView;
        private TextView _white1TextView;
        private TextView _white2TextView;
        private TextView _blueTextView;
        private TextView _battary1TextView;
        private TextView _battary2TextView;
        private TextView _robotTextView;
        private TextView _wall1TextView;
        private TextView _wall2TextView;
        private TextView _time1TextView;
        private TextView _time2TextView;
        private CheckBox _confirmCheckBox;
        private Button _saveButton;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_oml_sending_confirmation, container, false);
            _teamTextView = view.FindViewById<TextView>(Resource.Id.oml_sending_confirmation_team_text_view);
            _redTextView = view.FindViewById<TextView>(Resource.Id.oml_sending_confirmation_red_text_view);
            _yellowTextView = view.FindViewById<TextView>(Resource.Id.oml_sending_confirmation_yellow_text_view);
            _greenTextView = view.FindViewById<TextView>(Resource.Id.oml_sending_confirmation_green_text_view);
            _white1TextView = view.FindViewById<TextView>(Resource.Id.oml_sending_confirmation_white1_text_view);
            _white2TextView = view.FindViewById<TextView>(Resource.Id.oml_sending_confirmation_white2_text_view);
            _blueTextView = view.FindViewById<TextView>(Resource.Id.oml_sending_confirmation_blue_text_view);
            _battary1TextView = view.FindViewById<TextView>(Resource.Id.oml_sending_confirmation_battary1_text_view);
            _battary2TextView = view.FindViewById<TextView>(Resource.Id.oml_sending_confirmation_battary2_text_view);
            _robotTextView = view.FindViewById<TextView>(Resource.Id.oml_sending_confirmation_robot_text_view);
            _wall1TextView = view.FindViewById<TextView>(Resource.Id.oml_sending_confirmation_wall1_text_view);
            _wall2TextView = view.FindViewById<TextView>(Resource.Id.oml_sending_confirmation_wall2_text_view);
            _time1TextView = view.FindViewById<TextView>(Resource.Id.oml_sending_confirmation_time1_text_view);
            _time2TextView = view.FindViewById<TextView>(Resource.Id.oml_sending_confirmation_time2_text_view);
            _confirmCheckBox = view.FindViewById<CheckBox>(Resource.Id.oml_sending_confirmation_team_confirm_check_box);
            _confirmCheckBox.CheckedChange += ConfirmCheckBoxOnCheckedChange;
            _saveButton = view.FindViewById<Button>(Resource.Id.oml_saving_confirmation_save_button);
            _saveButton.Click += SaveButtonOnClick;
            var cancelButton = view.FindViewById<Button>(Resource.Id.oml_confirmation_cancel_button);
            cancelButton.Click += CancelButtonOnClick;

            _confirmCheckBox.Checked = savedInstanceState?.GetBoolean(ConfirmedTag, false) ?? false;

            SetViewModelValues();

            return view;
        }

        private void ConfirmCheckBoxOnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            _confirmed = checkedChangeEventArgs.IsChecked;
            _saveButton.Enabled = _confirmed;
        }

        private void CancelButtonOnClick(object sender, EventArgs eventArgs)
        {
            Dialog.Cancel();
        }

        private void SaveButtonOnClick(object sender, EventArgs eventArgs)
        {
            Confirmed?.Invoke(_vm);
            Dismiss();
        }

        public void SetViewModelData(OmlItemViewModel viewModel)
        {
            _vm = viewModel;
            SetViewModelValues();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutBoolean(ConfirmedTag, _confirmed);
            base.OnSaveInstanceState(outState);
        }

        private void SetViewModelValues()
        {
            if (_vm == null || _saveButton == null) return;

            var passengerStates = Application.Context.Resources.GetStringArray(Resource.Array.passenger_states);
            var blueStates = Application.Context.Resources.GetStringArray(Resource.Array.blue_block_state);
            var battaryStates = Application.Context.Resources.GetStringArray(Resource.Array.battary_state);
            var robotStates = Application.Context.Resources.GetStringArray(Resource.Array.robot_state);
            var wallStates = Application.Context.Resources.GetStringArray(Resource.Array.wall_state);

            _teamTextView.Text = Application.Context.GetString(Resource.String.team_format, _vm.TeamId);

            _redTextView.Text = Application.Context.GetString(Resource.String.red_format,
                passengerStates[_vm.RedBlockState + 1 ?? 0]);

            _yellowTextView.Text = Application.Context.GetString(Resource.String.yellow_format,
                passengerStates[_vm.YellowBlockState + 1 ?? 0]);

            _greenTextView.Text = Application.Context.GetString(Resource.String.green_format,
                passengerStates[_vm.GreenBlockState + 1 ?? 0]);

            _white1TextView.Text = Application.Context.GetString(Resource.String.white1_format,
                passengerStates[_vm.WhiteBlock1State + 1 ?? 0]);

            _white2TextView.Text = Application.Context.GetString(Resource.String.white2_format,
                passengerStates[_vm.WhiteBlock2State + 1 ?? 0]);

            if (_vm.BlueBlockState != null)
                _blueTextView.Text =
                    Application.Context.GetString(Resource.String.blue_format, blueStates[_vm.BlueBlockState.Value]);

            _battary1TextView.Text = Application.Context.GetString(Resource.String.battary1_format,
                battaryStates[_vm.BattaryBlock1State + 1 ?? 0]);

            _battary2TextView.Text = Application.Context.Resources.GetString(Resource.String.battary2_format,
                battaryStates[_vm.BattaryBlock2State + 1 ?? 0]);

            if (_vm.RobotState != null)
                _robotTextView.Text =
                    Application.Context.Resources.GetString(Resource.String.robot_format,
                        robotStates[_vm.RobotState.Value]);

            if (_vm.Wall1State != null)
                _wall1TextView.Text = Application.Context.Resources.GetString(Resource.String.wall1_format,
                    wallStates[_vm.Wall1State.Value]);

            if (_vm.Wall2State != null)
                _wall2TextView.Text =
                    Application.Context.Resources.GetString(Resource.String.wall2_format, wallStates[_vm.Wall2State.Value]);

            if (_vm.Time1 != null)
                _time1TextView.Text = Application.Context.Resources.GetString(Resource.String.time1_format,
                    new DateTime().AddMilliseconds(_vm.Time1.Value).ToString("mm:ss.ff"));

            if (_vm.Time2 != null)
                _time2TextView.Text = Application.Context.Resources.GetString(Resource.String.time2_format,
                    new DateTime().AddMilliseconds(_vm.Time2.Value).ToString("mm:ss.ff"));

        }

        public event ConfirmedEventHandler Confirmed;
    }
}
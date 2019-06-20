using System;
using System.Globalization;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using RROScoreBoard.ViewModels;

namespace RROScoreBoard.Fragments
{
    internal class OmlItemEditFragment : DialogFragment
    {
        public const string FragmentTag = "RROWebService.fragments.OMLITEMEDITFRAGMENT";

        public delegate void ItemEditedEventHandler(OmlItemViewModel newItem);

        public event ItemEditedEventHandler ItemEdited;

        private OmlItemViewModel _itemViewModel;

        private Spinner _redBlockSpinner;
        private Spinner _yellowBlockSpinner;
        private Spinner _greenBlockSpiner;
        private Spinner _whiteBlock1Spinner;
        private Spinner _whiteBlock2Spinner;
        private Spinner _battaryBlock1Spinner;
        private Spinner _battaryBlock2Spinner;
        private CheckBox _blueBlockCheckBox;
        private CheckBox _robotCheckBox;
        private CheckBox _wall1CheckBox;
        private CheckBox _wall2CheckBox;
        private EditText _time1EditText;
        private EditText _time2EditText;
        private TextView _teamIdTextView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_oml_item_edit, container, false);

            _teamIdTextView = view.FindViewById<TextView>(Resource.Id.oml_item_edit_team_id);

            _redBlockSpinner = view.FindViewById<Spinner>(Resource.Id.oml_item_edit_redblock_spinner);
            _redBlockSpinner.ItemSelected += RedBlockSpinnerOnItemSelected;

            _yellowBlockSpinner = view.FindViewById<Spinner>(Resource.Id.oml_item_edit_yellowblock_spinner);
            _yellowBlockSpinner.ItemSelected += YellowBlockSpinnerOnItemSelected;

            _greenBlockSpiner = view.FindViewById<Spinner>(Resource.Id.oml_item_edit_greenblock_spinner);
            _greenBlockSpiner.ItemSelected += GreenBlockSpinerOnItemSelected;

            _whiteBlock1Spinner = view.FindViewById<Spinner>(Resource.Id.oml_item_edit_whiteblock1_spinner);
            _whiteBlock1Spinner.ItemSelected += WhiteBlock1SpinnerOnItemSelected;

            _whiteBlock2Spinner = view.FindViewById<Spinner>(Resource.Id.oml_item_edit_whiteblock2_spinner);
            _whiteBlock2Spinner.ItemSelected += WhiteBlock2SpinnerOnItemSelected;

            _battaryBlock1Spinner = view.FindViewById<Spinner>(Resource.Id.oml_item_edit_battaryblock1_spinner);
            _battaryBlock1Spinner.ItemSelected += BattaryBlock1SpinnerOnItemSelected;

            _battaryBlock2Spinner = view.FindViewById<Spinner>(Resource.Id.oml_item_edit_battaryblock2_spinner);
            _battaryBlock2Spinner.ItemSelected += BattaryBlock2SpinnerOnItemSelected;

            _blueBlockCheckBox = view.FindViewById<CheckBox>(Resource.Id.oml_item_edit_blue_checkbox);
            _blueBlockCheckBox.CheckedChange += BlueBlockCheckBoxOnCheckedChange;

            _robotCheckBox = view.FindViewById<CheckBox>(Resource.Id.oml_item_edit_robot_checkbox);
            _robotCheckBox.CheckedChange += RobotCheckBoxOnCheckedChange;

            _wall1CheckBox = view.FindViewById<CheckBox>(Resource.Id.oml_item_edit_wall1_checkbox);
            _wall1CheckBox.CheckedChange += Wall1CheckBoxOnCheckedChange;

            _wall2CheckBox = view.FindViewById<CheckBox>(Resource.Id.oml_item_edit_wall2_checkbox);
            _wall2CheckBox.CheckedChange += Wall2CheckBoxOnCheckedChange;

            _time1EditText = view.FindViewById<EditText>(Resource.Id.oml_item_edit_time1_edittext);
            _time1EditText.TextChanged += TimeEditTextOnTextChanged;
            _time1EditText.KeyPress += TimeEditTextOnKeyPress;

            _time2EditText = view.FindViewById<EditText>(Resource.Id.oml_item_edit_time2_edittext);
            _time2EditText.TextChanged += TimeEditTextOnTextChanged;

            var saveButton = view.FindViewById<Button>(Resource.Id.oml_item_edit_save_button);
            saveButton.Click += SaveButtonOnClick;
            SetViewModelValues();
            return view;
        }

        private static void TimeEditTextOnKeyPress(object sender, View.KeyEventArgs keyEventArgs)
        {
            var editText = (EditText) sender;
            if (keyEventArgs.KeyCode >= Keycode.Num0 && keyEventArgs.KeyCode <= Keycode.Num9 && editText.Text.Length == 8)
                keyEventArgs.Handled = true;

            else keyEventArgs.Handled = false;

        }

        private void TimeEditTextOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var editText = (EditText) sender;
            
            var parsed = DateTime.TryParseExact(editText.Text, "mm:ss.ff", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var parsedTime);
            if (parsed)
            {
                if (editText.Id == Resource.Id.oml_item_edit_time1_edittext)
                    _itemViewModel.Time1 = (int) parsedTime.TimeOfDay.TotalMilliseconds;
                else _itemViewModel.Time2 = (int) parsedTime.TimeOfDay.TotalMilliseconds;
                return;
            }

            if (textChangedEventArgs.AfterCount == textChangedEventArgs.BeforeCount) return;

            if (textChangedEventArgs.AfterCount > textChangedEventArgs.BeforeCount)
            {
                var selectionPosition = editText.SelectionStart;
                var oldText = editText.Text;
                var text = editText.Text.Replace(":", "").Replace(".", "");
                if (text.Length >= 2) text = text.Insert(2, ":");
                if (text.Length >= 5) text = text.Insert(5, ".");
                editText.Text = text;
                if (oldText.Length == text.Length) editText.SetSelection(selectionPosition);
                else editText.SetSelection(selectionPosition + 1);
                return;
            }

            {
                var selectionPosition = editText.SelectionStart;
                var oldText = editText.Text;
                var text = editText.Text.Replace(":", "").Replace(".", "");
                if (text.Length > 2) text = text.Insert(2, ":");
                if (text.Length > 5) text = text.Insert(5, ".");
                editText.Text = text;
                if (oldText.Length == text.Length) editText.SetSelection(selectionPosition);
                else editText.SetSelection(selectionPosition != 0 ? selectionPosition - 1 : 0);
            }

        }

        private void Wall2CheckBoxOnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            if (_itemViewModel == null) return;
            _itemViewModel.Wall2State = checkedChangeEventArgs.IsChecked ? 1 : 0;
        }

        private void Wall1CheckBoxOnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            if (_itemViewModel == null) return;
            _itemViewModel.Wall1State = checkedChangeEventArgs.IsChecked ? 1 : 0;
        }

        private void RobotCheckBoxOnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            if (_itemViewModel == null) return;
            _itemViewModel.RobotState = checkedChangeEventArgs.IsChecked ? 1 : 0;
        }

        private void BlueBlockCheckBoxOnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            if (_itemViewModel == null) return;
            _itemViewModel.BlueBlockState = checkedChangeEventArgs.IsChecked ? 1 : 0;
        }

        private void BattaryBlock2SpinnerOnItemSelected(object sender, AdapterView.ItemSelectedEventArgs itemSelectedEventArgs)
        {
            if (_itemViewModel == null) return;
            _itemViewModel.BattaryBlock2State = itemSelectedEventArgs.Position == 0 ? null : (int?)itemSelectedEventArgs.Position - 1;
        }

        private void BattaryBlock1SpinnerOnItemSelected(object sender, AdapterView.ItemSelectedEventArgs itemSelectedEventArgs)
        {
            if (_itemViewModel == null) return;
            _itemViewModel.BattaryBlock1State = itemSelectedEventArgs.Position == 0 ? null : (int?)itemSelectedEventArgs.Position - 1;
        }

        private void WhiteBlock2SpinnerOnItemSelected(object sender, AdapterView.ItemSelectedEventArgs itemSelectedEventArgs)
        {
            if (_itemViewModel == null) return;
            _itemViewModel.WhiteBlock2State = itemSelectedEventArgs.Position == 0 ? null : (int?)itemSelectedEventArgs.Position - 1;
        }

        private void WhiteBlock1SpinnerOnItemSelected(object sender, AdapterView.ItemSelectedEventArgs itemSelectedEventArgs)
        {
            if (_itemViewModel == null) return;
            _itemViewModel.WhiteBlock1State = itemSelectedEventArgs.Position == 0 ? null : (int?)itemSelectedEventArgs.Position - 1;
        }

        private void GreenBlockSpinerOnItemSelected(object sender, AdapterView.ItemSelectedEventArgs itemSelectedEventArgs)
        {
            if (_itemViewModel == null) return;
            _itemViewModel.GreenBlockState = itemSelectedEventArgs.Position == 0 ? null : (int?)itemSelectedEventArgs.Position - 1;
        }

        private void YellowBlockSpinnerOnItemSelected(object sender, AdapterView.ItemSelectedEventArgs itemSelectedEventArgs)
        {
            if (_itemViewModel == null) return;
            _itemViewModel.YellowBlockState = itemSelectedEventArgs.Position == 0 ? null : (int?)itemSelectedEventArgs.Position - 1;
        }

        private void RedBlockSpinnerOnItemSelected(object sender, AdapterView.ItemSelectedEventArgs itemSelectedEventArgs)
        {
            if (_itemViewModel == null) return;
            _itemViewModel.RedBlockState = itemSelectedEventArgs.Position == 0 ? null : (int?)itemSelectedEventArgs.Position - 1;
        }

        private void SaveButtonOnClick(object sender, EventArgs eventArgs)
        {
            Dialog.Cancel();
        }

        public override void OnCancel(IDialogInterface dialog)
        {
            ItemEdited?.Invoke(_itemViewModel);
            base.OnCancel(dialog);
        }

        private void SetViewModelValues()
        {
            if (_itemViewModel == null || _time1EditText == null) return;

            _teamIdTextView.Text = _itemViewModel.TeamId;

            _redBlockSpinner.SetSelection(_itemViewModel.RedBlockState + 1 ?? 0);
            _yellowBlockSpinner.SetSelection(_itemViewModel.YellowBlockState + 1 ?? 0);
            _greenBlockSpiner.SetSelection(_itemViewModel.GreenBlockState + 1 ?? 0);
            _whiteBlock1Spinner.SetSelection(_itemViewModel.WhiteBlock1State + 1 ?? 0);
            _whiteBlock2Spinner.SetSelection(_itemViewModel.WhiteBlock2State + 1 ?? 0);
            _blueBlockCheckBox.Checked =
                _itemViewModel.BlueBlockState != null && _itemViewModel.BlueBlockState == 1;
            _battaryBlock1Spinner.SetSelection(_itemViewModel.BattaryBlock1State + 1 ?? 0);
            _battaryBlock2Spinner.SetSelection(_itemViewModel.BattaryBlock2State + 1 ?? 0);
            _robotCheckBox.Checked =
                _itemViewModel.RobotState != null && _itemViewModel.RobotState == 1;
            _wall1CheckBox.Checked =
                _itemViewModel.Wall1State != null && _itemViewModel.Wall1State == 1;
            _wall2CheckBox.Checked =
                _itemViewModel.Wall2State != null && _itemViewModel.Wall2State == 1;

            if (_itemViewModel.Time1 != null)
            {
                var time1 = new DateTime().AddMilliseconds(_itemViewModel.Time1.Value);
                _time1EditText.Text = time1.ToString("mm:ss.ff");
            }

            if (_itemViewModel.Time2 != null)
            {
                var time2 = new DateTime().AddMilliseconds(_itemViewModel.Time2.Value);
                _time2EditText.Text = time2.ToString("mm:ss.ff");
            }

            if (_itemViewModel.Saved == 1)
            {
                _redBlockSpinner.Enabled = false;
                _yellowBlockSpinner.Enabled = false;
                _greenBlockSpiner.Enabled = false;
                _whiteBlock1Spinner.Enabled = false;
                _whiteBlock2Spinner.Enabled = false;
                _battaryBlock1Spinner.Enabled = false;
                _battaryBlock2Spinner.Enabled = false;
                _blueBlockCheckBox.Enabled = false;
                _robotCheckBox.Enabled = false;
                _wall1CheckBox.Enabled = false;
                _wall2CheckBox.Enabled = false;
                _time1EditText.Enabled = false;
                _time2EditText.Enabled = false;
            }
        }

        public void SetItemData(OmlItemViewModel itemViewModel)
        {
            _itemViewModel = itemViewModel;
            SetViewModelValues();
        }
    }               
}
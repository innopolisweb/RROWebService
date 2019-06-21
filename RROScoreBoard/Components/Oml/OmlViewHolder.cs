using System;
using System.ComponentModel;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using RROScoreBoard.ViewModels;

namespace RROScoreBoard.Components.Oml
{
    internal class OmlViewHolder : RecyclerView.ViewHolder
    {
        public delegate void ItemSelectedEventHandler(int itemPosition);
        public delegate void SendEventHandler(int itemPosition);

        private OmlItemViewModel _vm;

        private readonly TextView _teamTextView;
        private readonly TextView _shortDataTextView;
        private readonly LinearLayout _loadingLayout;
        private readonly Button _sendButton;

        public OmlViewHolder(View itemView) : base(itemView)
        {
            _teamTextView = itemView.FindViewById<TextView>(Resource.Id.item_oml_sb_team_id);
            _shortDataTextView = itemView.FindViewById<TextView>(Resource.Id.item_oml_sb_short_text_data);
            _loadingLayout = itemView.FindViewById<LinearLayout>(Resource.Id.item_oml_sb_loading);
            _sendButton = itemView.FindViewById<Button>(Resource.Id.item_oml_sb_send_button);
            _sendButton.Enabled = false;

            _sendButton.Click += SendButtonOnClick;
            itemView.Click += ItemViewOnClick;
        }

        private void ItemViewOnClick(object sender, EventArgs eventArgs)
        {
            ItemSelected?.Invoke(AdapterPosition);
        }

        private void SendButtonOnClick(object o, EventArgs eventArgs)
        {
            Send?.Invoke(AdapterPosition);
        }

        public void ClearEventListeners()
        {
            ItemSelected = null;
            Send = null;
        }

        public void SetViewModel(OmlItemViewModel vm)
        {
            _vm?.ClearEventListeners();
            _vm = vm;
            _vm.PropertyChanged += VmOnPropertyChanged;
            OnTeamIdChanged();
            OnBusyChanged();
            OnDataChanged();
        }

        private void VmOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case nameof(_vm.TeamId):
                    OnTeamIdChanged();
                    break;
                case nameof(_vm.IsBusy):
                    OnBusyChanged();
                    break;
                case nameof(_vm.IsSending):
                    OnSendingChanged();
                    break;
                default:
                    OnDataChanged();
                    break;
            }
        }

        private void OnSendingChanged()
        {
            if (_vm.IsSending)
            {
                _sendButton.Enabled = false;
                _sendButton.Text = Application.Context.Resources.GetString(Resource.String.sending);
            }
            else
            {
                if (_vm.Saved == 1)
                {
                    _sendButton.Enabled = false;
                    _sendButton.Text = Application.Context.Resources.GetString(Resource.String.sent);
                }
                else
                {
                    _sendButton.Enabled = true;
                    _sendButton.Text = Application.Context.Resources.GetString(Resource.String.send);
                }
            }
            
        }

        private void OnBusyChanged()
        {
            if (_vm.IsBusy)
            {
                _shortDataTextView.Visibility = ViewStates.Gone;
                _loadingLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                _shortDataTextView.Visibility = ViewStates.Visible;
                _loadingLayout.Visibility = ViewStates.Gone;
                OnDataChanged();
            }
        }

        private void OnDataChanged()
        {
            if (_vm.IsBusy) return;

            var passengerStates = Application.Context.Resources.GetStringArray(Resource.Array.passenger_states);
            var blueBlockStates = Application.Context.Resources.GetStringArray(Resource.Array.blue_block_state);
            var robotStates = Application.Context.Resources.GetStringArray(Resource.Array.robot_state);
            var battaryStates = Application.Context.Resources.GetStringArray(Resource.Array.battary_state);
            var wallStates = Application.Context.Resources.GetStringArray(Resource.Array.wall_state);

            var finalText = "";

            if (_vm.RedBlockState != null)
                finalText += Application.Context.Resources.GetString(Resource.String.red_format,
                    passengerStates[_vm.RedBlockState.Value + 1]) + "; ";

            if (_vm.YellowBlockState != null)
                finalText += Application.Context.Resources.GetString(Resource.String.yellow_format,
                    passengerStates[_vm.YellowBlockState.Value + 1]) + "; ";

            if (_vm.GreenBlockState != null)
                finalText += Application.Context.Resources.GetString(Resource.String.green_format,
                    passengerStates[_vm.GreenBlockState.Value + 1]) + "; ";

            if (_vm.WhiteBlock1State != null)
                finalText += Application.Context.Resources.GetString(Resource.String.white1_format,
                    passengerStates[_vm.WhiteBlock1State.Value + 1]) + "; ";

            if (_vm.WhiteBlock2State != null)
                finalText += Application.Context.Resources.GetString(Resource.String.white2_format,
                    passengerStates[_vm.WhiteBlock2State.Value + 1]) + "; ";

            if (_vm.BlueBlockState != null)
                finalText += Application.Context.Resources.GetString(Resource.String.blue_format,
                    blueBlockStates[_vm.BlueBlockState.Value]) + "; ";

            if (_vm.BattaryBlock1State != null)
                finalText += Application.Context.Resources.GetString(Resource.String.battary1_format,
                    battaryStates[_vm.BattaryBlock1State.Value + 1]) + "; ";

            if (_vm.BattaryBlock2State != null)
                finalText += Application.Context.Resources.GetString(Resource.String.battary2_format,
                    battaryStates[_vm.BattaryBlock2State.Value + 1]) + "; ";

            if (_vm.RobotState != null)
                finalText += Application.Context.Resources.GetString(Resource.String.robot_format,
                    robotStates[_vm.RobotState.Value]) + "; ";

            if (_vm.Wall1State != null)
                finalText += Application.Context.Resources.GetString(Resource.String.wall1_format,
                    wallStates[_vm.Wall1State.Value]) + "; ";

            if (_vm.Wall2State != null)
                finalText += Application.Context.Resources.GetString(Resource.String.wall2_format,
                    wallStates[_vm.Wall2State.Value]) + "; ";

            if (_vm.Time1 != null)
                finalText += Application.Context.Resources.GetString(Resource.String.time1_format,
                    new DateTime().AddMilliseconds((int) _vm.Time1).ToString("mm:ss.ff")) + "; ";

            if (_vm.Time2 != null)
                finalText += Application.Context.Resources.GetString(Resource.String.time2_format,
                    new DateTime().AddMilliseconds((int) _vm.Time2).ToString("mm:ss.ff")) + "; ";

            finalText = finalText.Trim().ToLower();
            finalText = !String.IsNullOrEmpty(finalText)
                ? finalText.Remove(finalText.Length - 1)
                : "Tap here to change data";

            _shortDataTextView.Text = finalText;

            var hasNulls = _vm.RedBlockState == null || _vm.YellowBlockState == null || _vm.GreenBlockState == null ||
                           _vm.WhiteBlock1State == null || _vm.WhiteBlock2State == null || _vm.BlueBlockState == null ||
                           _vm.BattaryBlock1State == null || _vm.BattaryBlock2State == null || _vm.RobotState == null ||
                           _vm.Wall1State == null || _vm.Wall2State == null || _vm.Time1 == null || _vm.Time2 == null;

            _sendButton.Enabled = !hasNulls;

            if (_vm.Saved == 1)
            {
                _sendButton.Enabled = false;
                _sendButton.Text = Application.Context.Resources.GetString(Resource.String.sent);
            }
        }

        private void OnTeamIdChanged()
        {
            _teamTextView.Text = _vm.TeamId;
        }

        public event ItemSelectedEventHandler ItemSelected;

        public event SendEventHandler Send;

    }
}
using System.ComponentModel;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using RROScoreBoard.Components.Oml;
using RROScoreBoard.Services;
using RROScoreBoard.ViewModels;

namespace RROScoreBoard.Fragments
{
    public class OmlScoreBoardFragment : Fragment
    {
        public const string FragmentTag = "RROScoreBoard.fragments.OMLSCOREBOARDFRAGMENT";

        private const string SecondaryLoading = "RROScoreBoard.fragments.OMLSCOREBOARDFRAGMENT.SecondaryLoading";
        private const string SelectedItem = "RROScoreBoard.fragments.OMLSCOREBOARDFRAGMENT.SelectedItem";

        private OmlViewModel _vm;
        private ScoreBoardViewModel _boardViewModel;
        private RecyclerView _teamsRecyclerView;
        private int _selectedItem = -1;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _boardViewModel = ServiceProvider.GetService<ScoreBoardViewModel>();
            _vm = ServiceProvider.GetService<OmlViewModel>();
            _vm.PropertyChanged += VmOnPropertyChanged;

            if (savedInstanceState != null && savedInstanceState.ContainsKey(SelectedItem))
                _selectedItem = savedInstanceState.GetInt(SelectedItem);

            var editFragment =
                (OmlItemEditFragment) Activity.SupportFragmentManager.FindFragmentByTag(OmlItemEditFragment
                    .FragmentTag);
            if (editFragment != null)
            {
                editFragment.SetItemData(_vm.Teams[_selectedItem]);
                editFragment.ItemEdited += OnItemEdited;
            }

            var confirmationFragment =
                (OmlSendingConfirmationFragment) Activity.SupportFragmentManager.FindFragmentByTag(
                    OmlSendingConfirmationFragment.FragmentTag);
            if (confirmationFragment != null)
            {
                confirmationFragment.SetViewModelData(_vm.Teams[_selectedItem]);
                confirmationFragment.Confirmed += ConfirmationDialogOnConfirmed;
            }

        }

        private void VmOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case nameof(_vm.Teams):
                    OnTeamsChanged();
                    break;
            }
        }

        private void OnTeamsChanged()
        {
            var adapter = new OmlAdapter(_vm.Teams);

            adapter.ItemSelected += OnItemSelected;
            adapter.Send += OnItemSend;
            _teamsRecyclerView.SetAdapter(adapter);
        }

        private void OnItemSend(int itemPosition)
        {
            _selectedItem = itemPosition;
            var confirmationDialog = new OmlSendingConfirmationFragment();
            confirmationDialog.Confirmed += ConfirmationDialogOnConfirmed;
            confirmationDialog.SetViewModelData(_vm.Teams[itemPosition]);
            confirmationDialog.Show(Activity.SupportFragmentManager, OmlSendingConfirmationFragment.FragmentTag);
        }

        private void ConfirmationDialogOnConfirmed(OmlItemViewModel item)
        {
            item.Send(true);
        }

        private void OnItemSelected(int itemPosition)
        {
            _selectedItem = itemPosition;
            var editFragment = new OmlItemEditFragment();
            editFragment.ItemEdited += OnItemEdited;
            editFragment.SetItemData(_vm.Teams[itemPosition]);
            
            editFragment.Show(Activity.SupportFragmentManager, OmlItemEditFragment.FragmentTag);
        }

        private void OnItemEdited(OmlItemViewModel newItem)
        {

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_oml_scoreboard, container, false);
            _teamsRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.oml_sb_recyclerview);
            _teamsRecyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            _teamsRecyclerView.AddItemDecoration(new DividerItemDecoration(Context, DividerItemDecoration.Vertical));

            var secondary = savedInstanceState?.GetBoolean(SecondaryLoading, false) ?? false;

            if (secondary)
            {
                OnTeamsChanged();
            }
            else
            {
                _vm.LoadData(_boardViewModel.Teams);
            }

            return view;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutBoolean(SecondaryLoading, true);
            outState.PutInt(SelectedItem, _selectedItem);
            base.OnSaveInstanceState(outState);
        }


        public override void OnDestroy()
        {
            _vm.PropertyChanged -= VmOnPropertyChanged;
            base.OnDestroy();
        }
    }
}
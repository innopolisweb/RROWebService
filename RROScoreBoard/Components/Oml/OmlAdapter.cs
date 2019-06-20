using System.Collections.ObjectModel;
using Android.Support.V7.Widget;
using Android.Views;
using RROScoreBoard.ViewModels;

namespace RROScoreBoard.Components.Oml
{
    internal class OmlAdapter : RecyclerView.Adapter
    {
        public event OmlViewHolder.ItemSelectedEventHandler ItemSelected;

        public event OmlViewHolder.SendEventHandler Send;

        private readonly ObservableCollection<OmlItemViewModel> _teams;

        public OmlAdapter(ObservableCollection<OmlItemViewModel> teams)
        {
            _teams = teams;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var tHolder = (OmlViewHolder) holder;
            tHolder.ClearEventListeners();
            tHolder.SetViewModel(_teams[position]);
            tHolder.ItemSelected += itemPosition => ItemSelected?.Invoke(itemPosition);
            tHolder.Send += itemPosition => Send?.Invoke(itemPosition);
            
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var inflater = LayoutInflater.From(parent.Context);
            var view = inflater.Inflate(Resource.Layout.item_oml_scoreboard, parent, false);
            var holder = new OmlViewHolder(view);
            return holder;
        }

        public override int ItemCount => _teams.Count;
    }
}
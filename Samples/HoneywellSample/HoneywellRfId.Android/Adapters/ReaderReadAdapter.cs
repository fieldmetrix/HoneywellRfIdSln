using System;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace HoneywellRfId.Adapters
{
    public class ReaderReadAdapter : RecyclerView.Adapter
    {
        private ReaderViewHolder _holder;
        private Dictionary<string, string> _m;
        private Color[] _colors;
        private Color _selectedColor;
        private App _app;
        private List<Dictionary<string, string>> _showList = new List<Dictionary<string, string>>();

        private int _count;

        public ReaderReadAdapter()
        {

        }

        public ReaderReadAdapter(App app, List<Dictionary<string, string>> list)
        {
            _app = app;
            _showList = list;
            _colors = new Color[] { _app.Resources.GetColor(Resource.Color.tag_item_color1), _app.Resources.GetColor(Resource.Color.tag_item_color2) };
            _selectedColor = _app.Resources.GetColor(Resource.Color.tag_item_selected);
        }

        private string _scanMode = "0";
        private bool _itemCount = false;
        private bool _itemAnt = false;
        private bool _itemPro = false;
        private bool _itemRssi = false;
        private bool _itemFreq = false;
        private bool _itemData = false;
        private bool _itemAdditionType = false;
        private bool _itemTime = false;

        public void UpdateShowItems()
        {
            _scanMode =  "0";
            _itemCount = false;
            _itemAnt = false;
            _itemPro = false;
            _itemRssi = false;
            _itemFreq =  false;
            _itemData = false;
            _itemAdditionType = false;
            _itemTime = false;
        }

        // public override 
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(
                    _app).Inflate(Resource.Layout.listitemview_inv, parent,
                    false);

            _holder = new ReaderViewHolder(view);
            return _holder;
        }

        private Dictionary<string, string> FindTargetTag(List<Dictionary<string, string>> list)
        {
            for (int i = 0; i < list.Count(); i++)
            {
                if (_app.SelectedEpc.Equals(list[i][_app.Coname[1]]))
                {
                    return (Dictionary<string, string>)list[i];
                }
            }
            return null;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var holder = viewHolder as ReaderViewHolder;

            if (holder == null) return;

            if (_showList == null || _showList.Count() == 0)
            {
                return;
            }
            _m = (Dictionary<string, string>)_showList[position];
            holder.Item.SetBackgroundColor(_colors[position % 2]);
            holder.Item.Tag = position;
            holder.Readsort.Text = _m[_app.Coname[0]];
            holder.Readepc.Text = _m[_app.Coname[1]];
            holder.Readcnt.Text = _m[_app.Coname[2]];
            holder.Readant.Text = _m[_app.Coname[3]];
            holder.Readpro.Text = _m[_app.Coname[4]];
            holder.Readrssi.Text = _m[_app.Coname[5]];
            holder.Readfre.Text = _m[_app.Coname[6]];
            holder.Reademd.Text = _m[_app.Coname[7]];
            holder.Readetime.Text = _m[_app.Coname[8]];

            if (_scanMode.Equals("1"))
            {
                holder.Readcnt.Visibility = (_itemCount ? ViewStates.Visible : ViewStates.Gone);
                holder.Readant.Visibility = (_itemAnt ? ViewStates.Visible : ViewStates.Gone);
                holder.Readpro.Visibility = (_itemPro ? ViewStates.Visible : ViewStates.Gone);
                holder.Readrssi.Visibility = (_itemRssi ? ViewStates.Visible : ViewStates.Gone);
                holder.Readfre.Visibility = (_itemFreq ? ViewStates.Visible : ViewStates.Gone);
                bool showMd = _itemData && _itemAdditionType;
                holder.Reademd.Visibility = (showMd ? ViewStates.Visible : ViewStates.Gone);
                holder.Readetime.Visibility = (_itemTime ? ViewStates.Visible : ViewStates.Gone);
            }
            else
            {
                holder.Readcnt.Visibility = (ViewStates.Visible);
                holder.Readant.Visibility = (ViewStates.Gone);
                holder.Readpro.Visibility = (ViewStates.Gone);
                holder.Readrssi.Visibility = (ViewStates.Visible);
                holder.Readfre.Visibility = (ViewStates.Visible);
                holder.Reademd.Visibility = (ViewStates.Gone);
                holder.Readetime.Visibility = (ViewStates.Gone);
            }
            if (holder.Readepc.Text == _app.SelectedEpc)
            {
                holder.Item.SetBackgroundColor(_selectedColor);
            }
        }

        public override int ItemCount
        {
            get { return _showList.Count(); }
        }
    }

    public class ReaderViewHolder : RecyclerView.ViewHolder
    {
        public TextView Readsort { get; set; }
        public TextView Readepc { get; set; }
        public TextView Readcnt { get; set; }
        public TextView Readant { get; set; }
        public TextView Readpro { get; set; }
        public TextView Readrssi { get; set; }
        public TextView Readfre { get; set; }
        public TextView Reademd { get; set; }
        public TextView Readetime { get; set; }
        //   View.OnClickListener clickListener;

        public View Item { get; set; }

        public ReaderViewHolder(View view) : base(view)
        {
            Item = view;
            Readsort = (TextView)view.FindViewById(Resource.Id.textView_readsort);
            Readepc = (TextView)view.FindViewById(Resource.Id.textView_readepc);
            Readcnt = (TextView)view.FindViewById(Resource.Id.textView_readcnt);
            Readant = (TextView)view.FindViewById(Resource.Id.textView_readant);
            Readpro = (TextView)view.FindViewById(Resource.Id.textView_readpro);
            Readrssi = (TextView)view.FindViewById(Resource.Id.textView_readrssi);
            Readfre = (TextView)view.FindViewById(Resource.Id.textView_readfre);
            Reademd = (TextView)view.FindViewById(Resource.Id.textView_reademd);
            Readetime = (TextView)view.FindViewById(Resource.Id.textView_timestamp);
        }
    }
}
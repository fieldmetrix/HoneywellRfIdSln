using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using HoneywellRfidNative.Dto;

namespace HoneywellRfidNative.Adapters
{
    public class RfidTagListAdapter : BaseAdapter<TagDisplayItem>
    {
        private Dictionary<string, TagDisplayItem> _tags;
        private Context _context;

        public RfidTagListAdapter(Context context, ref Dictionary<string, TagDisplayItem> tags)
        {
            _context = context;
            _tags = tags;
        }

        public override TagDisplayItem this[int position]
        {
            get
            {
                return _tags.Values.ElementAt(position);
            }
        }

        public override int Count
        {
            get
            {
                return _tags.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.rfidlistview_item, parent, false);

                var epcTextView = view.FindViewById<TextView>(Resource.Id.rfidEpcTextView);
                var tagCountView = view.FindViewById<TextView>(Resource.Id.rfidTagCountTextView);

                view.Tag = new RfidViewHolder() { EpcView = epcTextView, TagCountView = tagCountView };
            }

            var holder = (RfidViewHolder)view.Tag;

            holder.EpcView.Text = _tags.Values.ElementAt(position).Epc;
            holder.TagCountView.Text = _tags.Values.ElementAt(position).Count.ToString();

            return view;
        }
    }

    public class RfidViewHolder : Java.Lang.Object
    {
        public TextView EpcView {get;set;}
        public TextView TagCountView { get; set; }
    }
}
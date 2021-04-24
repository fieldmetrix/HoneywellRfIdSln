﻿using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using HoneywellRfidNative.Dto;
using Android.Bluetooth;
using Android.Graphics;
using Com.Honeywell.Rfidservice;
using HoneywellRfIdShared.Hub;

namespace HoneywellRfidNative.Adapters
{

    public class RfidTagListAdapter : BaseAdapter<TagDisplayItem>
    {

        private Color _colorDefault;
        private Color _colorConnected;
        private Color _colorDisconnected;

        private Dictionary<string, TagDisplayItem> _tags;
        private Context _context;

        public RfidTagListAdapter(Context context, ref Dictionary<string, TagDisplayItem> tags)
        {

            _context = context;
            _tags = tags;

            _colorDefault = context.Resources.GetColor(Android.Resource.Color.BackgroundLight);
            _colorConnected = context.Resources.GetColor(Resource.Color.list_item_bg1);
            _colorDisconnected = context.Resources.GetColor(Resource.Color.list_item_bg2);

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

            View v;
            RfidViewHolder vh;

            if (convertView == null)
            {

                v = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.rfidlistview_item, parent, false);

                vh = new RfidViewHolder();
                vh.EpcView = (TextView)v.FindViewById(Resource.Id.rfidEpcTextView);
                vh.TagCountView = (TextView)v.FindViewById(Resource.Id.rfidTagCountTextView);
                v.Tag = vh;

            }
            else
            {
                v = convertView;
                vh = (RfidViewHolder)v.Tag;
            }

            var holder = (RfidViewHolder)v.Tag;

            holder.EpcView.Text = _tags.Values.ElementAt(position).Epc;
            holder.TagCountView.Text = _tags.Values.ElementAt(position).Count.ToString();

            v.SetBackgroundColor(_colorDefault);

            //BluetoothDevice device = GetItem(position) as BluetoothDevice;
            var item = GetItem(position);
            var propertyInfo = item.GetType().GetProperty("Instance");
            var test2 = (propertyInfo.GetValue(item, null) as TagDisplayItem);

            if (test2 != null)
            {
                if (test2.Epc == RfIdReaderActivity.SelectedEpc)
                {
                    v.SetBackgroundColor(_colorConnected);
                }
            }

            return v;

            #region old view code
            //var view = convertView;

            //if (view == null)
            //{
            //    view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.rfidlistview_item, parent, false);

            //    var epcTextView = view.FindViewById<TextView>(Resource.Id.rfidEpcTextView);
            //    var tagCountView = view.FindViewById<TextView>(Resource.Id.rfidTagCountTextView);

            //    view.Tag = new RfidViewHolder() { EpcView = epcTextView, TagCountView = tagCountView };
            //}

            //var holder = (RfidViewHolder)view.Tag;

            //holder.EpcView.Text = _tags.Values.ElementAt(position).Epc;
            //holder.TagCountView.Text = _tags.Values.ElementAt(position).Count.ToString();

            //return view;
            #endregion

        }

	}

    public class RfidViewHolder : Java.Lang.Object
    {
        public TextView EpcView {get;set;}
        public TextView TagCountView { get; set; }
    }
}
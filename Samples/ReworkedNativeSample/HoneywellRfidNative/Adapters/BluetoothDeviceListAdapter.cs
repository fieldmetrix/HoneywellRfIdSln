using System.Collections.Generic;
using Android.Bluetooth;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Com.Honeywell.Rfidservice;
using HoneywellRfIdShared.Hub;

namespace HoneywellRfidNative.Adapters
{
    public class BluetoothDeviceListAdapter : BaseAdapter<BluetoothDevice>
    {
        private Color _colorDefault;
        private Color _colorConnected;
        private Color _colorDisconnected;
        private Context _context;
        private Dictionary<string, int> _bleScanDevicesRssi = HoneywellDeviceHub.GetInstance().BleScanDevicesRssi;

        private List<BluetoothDevice> _items;

        public BluetoothDeviceListAdapter(Context context, ref List<BluetoothDevice> list)
        {
            _context = context;

            _items = list;

            _colorDefault = context.Resources.GetColor(Android.Resource.Color.BackgroundLight);
            _colorConnected = context.Resources.GetColor(Resource.Color.list_item_bg1);
            _colorDisconnected = context.Resources.GetColor(Resource.Color.list_item_bg2);
        }

        public override BluetoothDevice this[int position]
        {
            get
            {
                return _items[position];
            }
        }

        public override int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View v;
            ViewHolder vh;

            if (convertView == null)
            {
                v = LayoutInflater.From(_context).Inflate(
                        Resource.Layout.listview_item, parent, false);
                vh = new ViewHolder();
                vh.TvName = (TextView)v.FindViewById(Resource.Id.text1);
                vh.TvAddress = (TextView)v.FindViewById(Resource.Id.text2);
                vh.TvRssi = (TextView)v.FindViewById(Resource.Id.rssi);
                v.Tag = vh;
            }
            else
            {
                v = convertView;
                vh = (ViewHolder)v.Tag;
            }

            BluetoothDevice device = GetItem(position) as BluetoothDevice;
            vh.TvName.Text = device.Name != null ? device.Name : device.Address;
            vh.TvRssi.Text = _context.GetString(Resource.String.tv_rssi) + _bleScanDevicesRssi[device.Address];
            vh.TvAddress.Text = device.Address;
            v.SetBackgroundColor(_colorDefault);

            var selectedDev = HoneywellDeviceHub.GetInstance().SelectedBleDev;
            var rfIdMgr = HoneywellDeviceHub.GetInstance().RfidMgr;
            if (device.Equals(selectedDev))
            {
                if (rfIdMgr.ConnectionState == ConnectionState.StateConnected)
                {
                    vh.TvAddress.Text = _context.GetString(Resource.String.state_connected);
                    v.SetBackgroundColor(_colorConnected);
                }
                else if (rfIdMgr.ConnectionState == ConnectionState.StateConnecting)
                {
                    vh.TvAddress.Text = _context.GetString(Resource.String.state_connecting);
                    v.SetBackgroundColor(_colorDisconnected);
                }
                else if (rfIdMgr.ConnectionState == ConnectionState.StateDisconnected)
                {
                    vh.TvAddress.Text = device.Address;
                    v.SetBackgroundColor(_colorDisconnected);
                }
            }

            return v;
        }

        public void Clear()
        {
            _items.Clear();
        }
    }

    public class ViewHolder : Java.Lang.Object
    {
        public TextView TvName { get; set; }
        public TextView TvAddress { get; set; }
        public TextView TvRssi { get; set; }
    }
}
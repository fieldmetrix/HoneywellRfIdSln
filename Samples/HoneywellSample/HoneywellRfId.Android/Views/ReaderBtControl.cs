using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Honeywell.Rfidservice;
using HoneywellRfId.Adapters;
using HoneywellRfId.Broadcast;
using HoneywellRfId.Callbacks;
using HoneywellRfId.Consts;
using HoneywellRfId.Handlers;
using Java.Lang;
using static Android.Views.View;

namespace HoneywellRfId.Views
{
    [Activity(Label = "ReaderBtControl")]
    public class ReaderBtControl : BaseActivity, IOnClickListener, AdapterView.IOnItemClickListener, AdapterView.IOnItemLongClickListener
    { 
        private Switch _bTSwitch;
        private TextView _bTSeartTv;
        public Button AccessReadBtn;
        private View _view;
        private Switch _autoConnectSwitch;

        private Context _context;
        private App _app;

        private RfidManager _rfidMgr;
        private BluetoothAdapter _bluetoothAdapter;
        public List<BluetoothDevice> _bleScanDevices = App.GetInstance().BleScanDevices;
        private Dictionary<string, int> _bleScanDevicesRssi = App.GetInstance().BleScanDevicesRssi;
        private ListView _lvBleScan;
        public BluetoothDeviceListAdapter BleScanListAdapter;

        private List<ViewHolder> _test = new List<ViewHolder>();

        private BtReceiver _btReceiver;
        private ProgressDialog _progressDialog;
        public BtConnectionHandler ConnectionHandler;

        private const string KEY_MAC_ADDR = "mac_addr";
        private const string SP_NAME = "HoneywellRFID";
        private const string SP_KEY_AUTO_CONNECT = "auto_connect";
        private const int NORMAL_BT_SEARCH_TIME = 10000;
        private const int AUTO_CONNECT_BT_SEARCH_TIME = 3000;

        private BleScanCallBack _leScanCallback;

        private DateTime _prevListUpdateTime;

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.btn_search:
                    StartSearch();
                    break;
                case Resource.Id.access_read:
                    OnClickCreateReader();
                    break;
                default:
                    break;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            //  invalidateOptionsMenu();
            _rfidMgr.DeviceConnected += _rfidMgr_DeviceConnected;
            _rfidMgr.DeviceDisconnected += _rfidMgr_DeviceDisconnected;
            _rfidMgr.ReaderCreated += _rfidMgr_ReaderCreated;

            AccessReadBtn.Enabled = IsConnected();
            BleScanListAdapter.NotifyDataSetChanged();
        }

        private void _rfidMgr_ReaderCreated(object sender, ReaderCreatedEventArgs e)
        {

            if (e.Success)
            {
                _app.RfIdRdr = e.Reader;
                ConnectionHandler.SendEmptyMessage(BleMsgConsts.MSG_CREATE_READER_SUCCESSFULLY);
                Intent intent = new Intent(this, typeof(ReaderMain));
                StartActivity(intent);
            }
            else
            {
                ConnectionHandler.SendEmptyMessage(BleMsgConsts.MSG_CREATE_READER_FAILED);
            }
        }

        private void _rfidMgr_DeviceDisconnected(object sender, DeviceDisconnectedEventArgs e)
        {
            AccessReadBtn.Enabled = false;
            BleScanListAdapter.NotifyDataSetChanged();
            ConnectionHandler.RemoveMessages(BleMsgConsts.MSG_DEALY_CREATE_READER);
        }

        protected override void OnPause()
        {
            base.OnPause();

            _rfidMgr.DeviceConnected -= _rfidMgr_DeviceConnected;
            _rfidMgr.DeviceDisconnected -= _rfidMgr_DeviceDisconnected;
            _rfidMgr.ReaderCreated -= _rfidMgr_ReaderCreated;

           // _rfidMgr.RemoveEventListener(mEventListner);
        }

        private void _rfidMgr_DeviceConnected(object sender, DeviceConnectedEventArgs e)
        {
            BleScanListAdapter.NotifyDataSetChanged();
            ConnectionHandler.SendEmptyMessageDelayed(BleMsgConsts.MSG_DEALY_CREATE_READER, 1000);
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            ListView lv = (ListView)parent;
            BluetoothDevice device = (BluetoothDevice)lv.Adapter.GetItem(position);
            StopSearch();

            if (!IsConnected() || device != GetSelectedDev())
            {
                Disconnect();

                Connect(device.Address);
                SetSelectedDev(device);
                view.SetBackgroundColor(Color.Yellow);
                BleScanListAdapter.NotifyDataSetChanged();
            }
        }

        public bool OnItemLongClick(AdapterView parent, View view, int position, long id)
        {
            throw new NotImplementedException();
        }



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.bt_control_main);
            //            initToolBar();
            Init();
        }

        private void Init()
        {
            _context = this;
            _app = (App)Application;
            _rfidMgr = _app.RfidMgr;

            _view = FindViewById(Resource.Id.group);
            _bTSwitch = (Switch)FindViewById(Resource.Id.bt_switch);
            _autoConnectSwitch = (Switch)FindViewById(Resource.Id.auto_connect_switch);
            _bTSeartTv = (TextView)FindViewById(Resource.Id.btn_search);
            _lvBleScan = (ListView)FindViewById(Resource.Id.bt_list);
            AccessReadBtn = (Button)FindViewById(Resource.Id.access_read);
            _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            _bTSeartTv.SetOnClickListener(this);
            AccessReadBtn.SetOnClickListener(this);
            _bTSwitch.Checked = _bluetoothAdapter.IsEnabled;

            _bTSwitch.CheckedChange += _bTSwitch_CheckedChange;

            var sp = this.GetSharedPreferences(SP_NAME, FileCreationMode.Private);
            bool autoConnectState = sp.GetBoolean(SP_KEY_AUTO_CONNECT, false);
            _autoConnectSwitch.Checked = autoConnectState;

            _autoConnectSwitch.CheckedChange += _autoConnectSwitch_CheckedChange;

         //   _test.Add(new ViewHolder());
             BleScanListAdapter = new BluetoothDeviceListAdapter(this, ref _bleScanDevices);
            _lvBleScan.Adapter = BleScanListAdapter;
            _lvBleScan.OnItemClickListener = this;
            _lvBleScan.OnItemLongClickListener = this;

            _progressDialog = new ProgressDialog(this);
            _progressDialog.SetMessage(GetString(Resource.String.loading_text));
            ConnectionHandler = new BtConnectionHandler(this);
            _leScanCallback = new BleScanCallBack(ConnectionHandler);

            SetVisibility();

            _btReceiver = new BtReceiver();
            _btReceiver.OnBtStateChange += _btReceiver_OnBtStateChange;
            IntentFilter filter = new IntentFilter();
            filter.AddAction("android.bluetooth.adapter.action.STATE_CHANGED");
            RegisterReceiver(_btReceiver, filter);
        }

        private void _autoConnectSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var sp = GetSharedPreferences(SP_NAME,FileCreationMode.Private);
            ISharedPreferencesEditor editor = sp.Edit();
            editor.PutBoolean(SP_KEY_AUTO_CONNECT, e.IsChecked);
            editor.Commit();
        }

        private void _bTSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                if (!_bluetoothAdapter.IsEnabled)
                {
                    _bluetoothAdapter.Enable();
                }
            }
            else
            {
                StopSearch();
                _bluetoothAdapter.Disable();
                AccessReadBtn.Enabled = false;
            }
        }

        private void _btReceiver_OnBtStateChange(object sender, Events.BtStateChangedEventArgs e)
        {
            BleScanListAdapter.Clear();

            if (e.BtState == Android.Bluetooth.State.On)
            {
                _bTSwitch.SetOnCheckedChangeListener(null);
                _bTSwitch.Checked = true;
                _bTSwitch.CheckedChange -= _bTSwitch_CheckedChange;


                SetVisibility();
            }
            else if (e.BtState == Android.Bluetooth.State.Off)
            {
                _bTSwitch.SetOnCheckedChangeListener(null);
                _bTSwitch.Checked = false;
                _bTSwitch.CheckedChange += _bTSwitch_CheckedChange;
                SetVisibility();

                AccessReadBtn.Enabled = false;
            }
        }

        private void SetVisibility()
        {
            if (_bluetoothAdapter.IsEnabled)
            {
                _view.Visibility = ViewStates.Visible;
            }
            else
            {
                _view.Visibility = ViewStates.Gone;
            }
        }

        private void StartSearch()
        {
            SetSelectedDev(null);
            _bleScanDevices.Clear();
            _bleScanDevicesRssi.Clear();
            BleScanListAdapter.Clear();
            Disconnect();

            bool ret = _bluetoothAdapter.StartLeScan(_leScanCallback);
            _bTSeartTv.Text = (GetString(Resource.String.searching));
            _bTSeartTv.Enabled = false;

            ConnectionHandler.SendEmptyMessageDelayed(BleMsgConsts.MSG_BT_SCAN_TIMEOUT, _autoConnectSwitch.Checked ? AUTO_CONNECT_BT_SEARCH_TIME : NORMAL_BT_SEARCH_TIME);

        }

        private void SetSelectedDev(BluetoothDevice dev)
        {
            App.GetInstance().SelectedBleDev = dev;
        }

        private void Connect(string mac)
        {
            _rfidMgr.SetAutoReconnect(true);
            _rfidMgr.Connect(mac);
        }

        private void Disconnect()
        {
            AccessReadBtn.Enabled = false;
            _rfidMgr.Disconnect();
        }

        public void HandleMessage(Android.OS.Message msg)
        {
            switch (msg.What)
            {
                case BleMsgConsts.MSG_FINDING_BLE_SERVICES:
                    _progressDialog.SetTitle(GetString(Resource.String.finding_ble_service));
                    _progressDialog.Show();
                    break;
                case BleMsgConsts.MSG_CREATING_READER:
                    _progressDialog.SetTitle(GetString(Resource.String.creating_reader));
                    _progressDialog.Show();
                    break;
                case BleMsgConsts.MSG_CREATE_READER_SUCCESSFULLY:
                    _progressDialog.Dismiss();
                    Toast.MakeText(this, GetString(Resource.String.toast_create_reader_successfully), ToastLength.Short).Show();
                    break;
                case BleMsgConsts.MSG_CREATE_READER_FAILED:
                    _progressDialog.Dismiss();
                    Toast.MakeText(this, GetString(Resource.String.toast_create_reader_failed), ToastLength.Short).Show();
                    break;
                case BleMsgConsts.MSG_BT_SCAN_TIMEOUT:
                    SearchTimeout();
                    break;
                case BleMsgConsts.MSG_ON_BLE_DEV_FOUND:
                    int rssi = msg.Arg1;
                    BluetoothDevice device = (BluetoothDevice)msg.Obj;

                    if (!_bleScanDevices.Any(x => x.Address == device.Address))
                    {
                        _bleScanDevices.Add(device);
                        BleScanListAdapter.NotifyDataSetChanged();
                    }

                    _bleScanDevicesRssi[device.Address] =  rssi;
                    
                    DateTime cur = DateTime.Now;

                    var diffTime = cur - _prevListUpdateTime;

                    if (diffTime.TotalMilliseconds > 250)
                    {
                        BleScanListAdapter.NotifyDataSetChanged();
                        _prevListUpdateTime = cur;
                    }
                    break;
                case BleMsgConsts.MSG_DEALY_CREATE_READER:
                    ReadyForCreateReader();
                    break;
            }
        }

        private void SearchTimeout()
        {
            StopSearch();

            if (_autoConnectSwitch.Checked)
            {
                AutoConnect();
            }
        }


        private void ReadyForCreateReader()
        {
            string v = null;

            for (int i = 0; i < 3; i++)
            {
                v = _app.RfidMgr.BluetoothModuleSwVersion;

                if (!string.IsNullOrEmpty(v))
                {
                    if (v.CompareTo("2.0.0") <= 0)
                    {
                        Toast.MakeText(this, "Bluetooth firmware (" + v + ") is too old,\n\nPlease upgrade to new version.", ToastLength.Short).Show();

                        break;
                    }
                }

                try
                {
                    System.Threading.Thread.Sleep(30);
                }
                catch (InterruptedException e)
                {
                    e.PrintStackTrace();
                }
            }

            AccessReadBtn.Enabled = true;
        }

        private void StopSearch()
        {
            ConnectionHandler.RemoveMessages(BleMsgConsts.MSG_BT_SCAN_TIMEOUT);
            _bluetoothAdapter.StopLeScan(_leScanCallback);
            _bTSeartTv.Text = GetString(Resource.String.search);
            _bTSeartTv.Enabled = true;
        }

        private void OnClickCreateReader()
        {
            //                if (Common.DEVICE_AUTHENTICATION && RfidManager.getInstance(this).getState() != RfidManager.STATE_READY) {
            //                    Toast.makeText(this, getString(R.string.dev_not_certified), Toast.LENGTH_SHORT).show();
            //                } else
            if (IsConnected())
            {
                _rfidMgr.CreateReader();
                ConnectionHandler.SendEmptyMessage(BleMsgConsts.MSG_CREATING_READER);
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.bluetooth_not_connected), ToastLength.Short).Show();
            }
        }

        private bool IsConnected()
        {
            return GetCnntState() == ConnectionState.StateConnected;
        }

        private BluetoothDevice GetSelectedDev()
        {
            return App.GetInstance().SelectedBleDev;
        }

        private ConnectionState GetCnntState()
        {
            return _rfidMgr.ConnectionState;
        }

        private void AutoConnect()
        {
            if (_bleScanDevices.Count > 0)
            {
                var set = _bleScanDevicesRssi.ToHashSet();
                int min = int.MinValue;


                string mac = null;

                foreach (var entry in set)
                {
                    if (entry.Value > min)
                    {
                        min = entry.Value;
                        mac = entry.Key;
                    }
                }

                foreach (var dev in _bleScanDevices)
                {
                    if (dev.Address == mac)
                    {
                        SetSelectedDev(dev);
                        Connect(mac);
                        BleScanListAdapter.NotifyDataSetChanged();
                        return;
                    }
                }
            }
        }


    }
}
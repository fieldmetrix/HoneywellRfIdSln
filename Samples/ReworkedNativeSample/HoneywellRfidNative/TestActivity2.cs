using System;
using System.Collections.Generic;
using System.Linq;
using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Com.Honeywell.Rfidservice;
using HoneywellRfidNative.Adapters;
using HoneywellRfidNative.Broadcast;
using HoneywellRfidNative.Callbacks;
using HoneywellRfidNative.Consts;
using HoneywellRfidNative.Handlers;
using HoneywellRfIdShared.Hub;
using static Android.Views.View;

namespace HoneywellRfidNative
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class TestActivity2 : AppCompatActivity, IOnClickListener, AdapterView.IOnItemClickListener
    {
        private BleScanCallBack _leScanCallback;
        private BluetoothAdapter _bluetoothAdapter;
        private ProgressDialog _progressDialog;
        private List<BluetoothDevice> _bleScanDevices = new List<BluetoothDevice>();
        private BluetoothDeviceListAdapter _bleScanListAdapter;
        private Dictionary<string, int> _bleScanDevicesRssi = HoneywellDeviceHub.GetInstance().BleScanDevicesRssi;
        private BtReceiver _btReceiver;
        private BtConnectionHandler _connectionHandler;
        private RfidManager _rfidManager;

        private Button _bTSeartTv;
        private Button _accessReadBtn;
        private ListView _lvBleScan;

        private Context _context;

        private DateTime _prevListUpdateTime;

        private string[] _permissions = new string[]{Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.WriteExternalStorage};

        private List<string> _dismissPermissionList = new List<string>();

        private const int PERMISSION_REQUEST_CODE = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.testactivity2);

            Init();

            RequestDynamicPermissions();
        }

        private void Init()
        {
            _context = this;
            //   _app = (App)Application;
            _rfidManager = HoneywellDeviceHub.GetInstance().RfidMgr;


            _bTSeartTv = (Button)FindViewById(Resource.Id.btn_search);
            _lvBleScan = (ListView)FindViewById(Resource.Id.bt_list);
            _accessReadBtn = (Button)FindViewById(Resource.Id.access_read);
            _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;



            _bTSeartTv.SetOnClickListener(this);
            _bTSeartTv.Enabled = true;
            _accessReadBtn.SetOnClickListener(this);

            _bleScanListAdapter = new BluetoothDeviceListAdapter(this, ref _bleScanDevices);
            _lvBleScan.Adapter = _bleScanListAdapter;
            _lvBleScan.OnItemClickListener = this;

            _progressDialog = new ProgressDialog(this);
            _progressDialog.SetMessage(GetString(Resource.String.loading_text));
            _connectionHandler = new BtConnectionHandler(this);
            _leScanCallback = new BleScanCallBack(_connectionHandler);

            _btReceiver = new BtReceiver();
            _btReceiver.OnBtStateChange += _btReceiver_OnBtStateChange;
            IntentFilter filter = new IntentFilter();
            filter.AddAction("android.bluetooth.adapter.action.STATE_CHANGED");
            RegisterReceiver(_btReceiver, filter);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void HandleMessage(Android.OS.Message msg)
        {
            switch (msg.What)
            {
                case BleMsgConsts.MSG_FINDING_BLE_SERVICES:
                    _progressDialog.SetTitle("Finding Devices");
                    _progressDialog.Show();
                    break;
                case BleMsgConsts.MSG_CREATING_READER:
                    _progressDialog.SetTitle("Creating Reader");
                    _progressDialog.Show();
                    break;
                case BleMsgConsts.MSG_CREATE_READER_SUCCESSFULLY:
                    _progressDialog.Dismiss();
                    Toast.MakeText(this, "Created Reader", ToastLength.Short).Show();
                    break;
                case BleMsgConsts.MSG_CREATE_READER_FAILED:
                    _progressDialog.Dismiss();
                    Toast.MakeText(this, "Reader Failed", ToastLength.Short).Show();
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
                        _bleScanListAdapter.NotifyDataSetChanged();
                    }

                    _bleScanDevicesRssi[device.Address] = rssi;

                    DateTime cur = DateTime.Now;

                    var diffTime = cur - _prevListUpdateTime;

                    if (diffTime.TotalMilliseconds > 250)
                    {
                        _bleScanListAdapter.NotifyDataSetChanged();
                        _prevListUpdateTime = cur;
                    }
                    break;
                case BleMsgConsts.MSG_DEALY_CREATE_READER:
                    _accessReadBtn.Enabled = true;
                    break;
            }
        }

        private async void StartSearch()
        {
            SetSelectedDev(null);
            _bleScanDevices.Clear();
            _bleScanDevicesRssi.Clear();
            _bleScanListAdapter.Clear();
            Disconnect();

            bool ret = _bluetoothAdapter.StartLeScan(_leScanCallback);
            _bTSeartTv.Text = (GetString(Resource.String.searching));
            _bTSeartTv.Enabled = false;

            _connectionHandler.SendEmptyMessageDelayed(BleMsgConsts.MSG_BT_SCAN_TIMEOUT, 5000);

        }

        private void StopSearch()
        {
            _connectionHandler.RemoveMessages(BleMsgConsts.MSG_BT_SCAN_TIMEOUT);
            _bluetoothAdapter.StopLeScan(_leScanCallback);
            _bTSeartTv.Text = GetString(Resource.String.search);
            _bTSeartTv.Enabled = true;
        }


        private void SearchTimeout()
        {
            StopSearch();

        }

        private void _btReceiver_OnBtStateChange(object sender, Events.BtStateChangedEventArgs e)
        {
            _bleScanListAdapter.Clear();
            _accessReadBtn.Enabled = false;
        }


        private void SetSelectedDev(BluetoothDevice dev)
        {
            HoneywellDeviceHub.GetInstance().SelectedBleDev = dev;
        }

        private void Disconnect()
        {
            _accessReadBtn.Enabled = false;
            _rfidManager.Disconnect();
        }


        private void Connect(string mac)
        {
            _rfidManager.SetAutoReconnect(true);
            _rfidManager.Connect(mac);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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
                _bleScanListAdapter.NotifyDataSetChanged();
            }
        }

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

        private void OnClickCreateReader()
        {
            if (IsConnected())
            {
                _rfidManager.CreateReader();
                _connectionHandler.SendEmptyMessage(BleMsgConsts.MSG_CREATING_READER);
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.bluetooth_not_connected), ToastLength.Short).Show();
            }
        }

        private BluetoothDevice GetSelectedDev()
        {
            return HoneywellDeviceHub.GetInstance().SelectedBleDev;
        }

        private bool IsConnected()
        {
            return GetConnectionState() == ConnectionState.StateConnected;
        }

        private ConnectionState GetConnectionState()
        {
            return _rfidManager.ConnectionState;
        }

        private void RequestDynamicPermissions()
        {
            for (int i = 0; i < _permissions.Length; i++)
            {
                if (ContextCompat.CheckSelfPermission(this, _permissions[i]) !=
                        Android.Content.PM.Permission.Granted)
                {
                    _dismissPermissionList.Add(_permissions[i]);
                }
            }
            if (_dismissPermissionList.Count > 0)
            {
                ActivityCompat.RequestPermissions(this, _permissions, PERMISSION_REQUEST_CODE);
            }
        }

        //LIFE CYCLE CONTROL
        protected override void OnResume()
        {
            base.OnResume();

            _rfidManager.DeviceConnected += RfidMgr_DeviceConnected;
            _rfidManager.DeviceDisconnected += RfidMgr_DeviceDisconnected;
            _rfidManager.ReaderCreated += RfidMgr_ReaderCreated;

            _accessReadBtn.Enabled = IsConnected();
            _bleScanListAdapter.NotifyDataSetChanged();
        }

        protected override void OnPause()
        {
            base.OnPause();

            _rfidManager.DeviceConnected -= RfidMgr_DeviceConnected;
            _rfidManager.DeviceDisconnected -= RfidMgr_DeviceDisconnected;
            _rfidManager.ReaderCreated -= RfidMgr_ReaderCreated;
        }

        private void RfidMgr_DeviceConnected(object sender, DeviceConnectedEventArgs e)
        {
            _bleScanListAdapter.NotifyDataSetChanged();
            _connectionHandler.SendEmptyMessageDelayed(BleMsgConsts.MSG_DEALY_CREATE_READER, 1000);
        }

        private void RfidMgr_DeviceDisconnected(object sender, DeviceDisconnectedEventArgs e)
        {
            _accessReadBtn.Enabled = false;
            _bleScanListAdapter.NotifyDataSetChanged();
            _connectionHandler.RemoveMessages(BleMsgConsts.MSG_DEALY_CREATE_READER);
        }

        private void RfidMgr_ReaderCreated(object sender, ReaderCreatedEventArgs e)
        {

            if (e.Success)
            {
                HoneywellDeviceHub.GetInstance().RfIdRdr = e.Reader;
                _connectionHandler.SendEmptyMessage(BleMsgConsts.MSG_CREATE_READER_SUCCESSFULLY);
                Intent intent = new Intent(this, typeof(RfIdReaderActivity));
                StartActivity(intent);
            }
            else
            {
                _connectionHandler.SendEmptyMessage(BleMsgConsts.MSG_CREATE_READER_FAILED);
            }
        }
    }
}

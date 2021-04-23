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

	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
	public class TestActivity1 : AppCompatActivity
	{

		// IH25 - 1
		static string mac1 = "0C:23:69:19:5E:3D";

		// IH25 - 2
		static string mac2 = "0C:23:69:19:6A:44";

		static BluetoothDevice btRfidScannerDevice = null;

		private Context _context;

		Button btnBTScan;
		Button btnInit;
		Button btnScanTags;
		private BtReceiver _btReceiver;
		private BtBroadcastReceiver _receiver;
		private BluetoothAdapter _bluetoothAdapter = null;
		private RfidManager _rfidManager;

		private string[] _permissions = new string[]{Manifest.Permission.AccessCoarseLocation,
			Manifest.Permission.WriteExternalStorage};

		private List<string> _dismissPermissionList = new List<string>();

		private const int PERMISSION_REQUEST_CODE = 1;

		protected override void OnCreate(Bundle savedInstanceState)
		{

			base.OnCreate(savedInstanceState);

			Xamarin.Essentials.Platform.Init(this, savedInstanceState);

			SetContentView(Resource.Layout.testactivity1);

			btnBTScan = FindViewById<Button>(Resource.Id.btnBTScan);
			btnBTScan.Click += BtnBTScan_Click;

			btnInit = FindViewById<Button>(Resource.Id.btnInit);
			btnInit.Enabled = false;
			btnInit.Click += BtnInit_Click;

			btnScanTags = FindViewById<Button>(Resource.Id.btnScanTags);
			btnScanTags.Enabled = false;
			btnScanTags.Click += BtnScanTags_Click;

			Init();

			RequestDynamicPermissions();

		}

		private void Init()
		{

			_context = this;
			
			_rfidManager = HoneywellDeviceHub.GetInstance().RfidMgr;
			_bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
			//SetSelectedDev(null);
			//Disconnect();

			_btReceiver = new BtReceiver();
			IntentFilter filter = new IntentFilter();
			filter.AddAction("android.bluetooth.adapter.action.STATE_CHANGED");
			RegisterReceiver(_btReceiver, filter);

			_receiver = new BtBroadcastReceiver();
			RegisterReceiver(_receiver, new IntentFilter(BluetoothDevice.ActionFound));

		}

		//public void HandleMessage(Android.OS.Message msg)
		//{
		//	switch (msg.What)
		//	{
		//		case BleMsgConsts.MSG_FINDING_BLE_SERVICES:
		//			//_progressDialog.SetTitle("Finding Devices");
		//			//_progressDialog.Show();
		//			break;
		//		case BleMsgConsts.MSG_CREATING_READER:
		//			//_progressDialog.SetTitle("Creating Reader");
		//			//_progressDialog.Show();
		//			break;
		//		case BleMsgConsts.MSG_CREATE_READER_SUCCESSFULLY:
		//			//_progressDialog.Dismiss();
		//			Toast.MakeText(this, "Created Reader", ToastLength.Short).Show();
		//			break;
		//		case BleMsgConsts.MSG_CREATE_READER_FAILED:
		//			//_progressDialog.Dismiss();
		//			Toast.MakeText(this, "Reader Failed", ToastLength.Short).Show();
		//			break;
		//		case BleMsgConsts.MSG_BT_SCAN_TIMEOUT:
		//			//SearchTimeout();
		//			break;
		//		case BleMsgConsts.MSG_ON_BLE_DEV_FOUND:
		//			//int rssi = msg.Arg1;
		//			//BluetoothDevice device = (BluetoothDevice)msg.Obj;

		//			//if (!_bleScanDevices.Any(x => x.Address == device.Address))
		//			//{
		//			//	_bleScanDevices.Add(device);
		//			//	_bleScanListAdapter.NotifyDataSetChanged();
		//			//}

		//			//_bleScanDevicesRssi[device.Address] = rssi;

		//			//DateTime cur = DateTime.Now;

		//			//var diffTime = cur - _prevListUpdateTime;

		//			//if (diffTime.TotalMilliseconds > 250)
		//			//{
		//			//	_bleScanListAdapter.NotifyDataSetChanged();
		//			//	_prevListUpdateTime = cur;
		//			//}
		//			break;
		//		case BleMsgConsts.MSG_DEALY_CREATE_READER:
		//			//_accessReadBtn.Enabled = true;
		//			break;
		//	}
		//}

		private void _receiver_DeviceReceived(object sender, EventArgs e)
		{
			var newBTDevice = sender as BluetoothDevice;
			if (newBTDevice.Address == mac1 || newBTDevice.Address == mac2)
			{
				btRfidScannerDevice = newBTDevice;
				btnInit.Enabled = true;
			}
		}

		private void _btReceiver_OnBtStateChange(object sender, Events.BtStateChangedEventArgs e)
		{
			btnScanTags.Enabled = false;
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

		private void SetSelectedDev(BluetoothDevice dev)
		{
			HoneywellDeviceHub.GetInstance().SelectedBleDev = dev;
		}

		private void Disconnect()
		{
			btnScanTags.Enabled = false;
			_rfidManager.Disconnect();
		}

		private void Connect(string mac)
		{
			_rfidManager.SetAutoReconnect(true);
			_rfidManager.Connect(mac);
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

		protected override void OnResume()
		{

			base.OnResume();

			_rfidManager.DeviceConnected += RfidMgr_DeviceConnected;
			_rfidManager.DeviceDisconnected += RfidMgr_DeviceDisconnected;
			_rfidManager.ReaderCreated += RfidMgr_ReaderCreated;

			if (_btReceiver != null)
				_btReceiver.OnBtStateChange += _btReceiver_OnBtStateChange;

			if (_receiver != null)
				_receiver.DeviceReceived += _receiver_DeviceReceived;

		}

		protected override void OnPause()
		{

			base.OnPause();

			_rfidManager.DeviceConnected -= RfidMgr_DeviceConnected;
			_rfidManager.DeviceDisconnected -= RfidMgr_DeviceDisconnected;
			_rfidManager.ReaderCreated -= RfidMgr_ReaderCreated;

			if (_btReceiver != null)
				_btReceiver.OnBtStateChange -= _btReceiver_OnBtStateChange;

			if (_receiver != null)
				_receiver.DeviceReceived -= _receiver_DeviceReceived;

		}

		private void RfidMgr_DeviceConnected(object sender, DeviceConnectedEventArgs e)
		{
			//_bleScanListAdapter.NotifyDataSetChanged();
			//_connectionHandler.SendEmptyMessageDelayed(BleMsgConsts.MSG_DEALY_CREATE_READER, 1000);
		}

		private void RfidMgr_DeviceDisconnected(object sender, DeviceDisconnectedEventArgs e)
		{
			btnScanTags.Enabled = false;
			//_bleScanListAdapter.NotifyDataSetChanged();
			//_connectionHandler.RemoveMessages(BleMsgConsts.MSG_DEALY_CREATE_READER);
		}

		private void RfidMgr_ReaderCreated(object sender, ReaderCreatedEventArgs e)
		{

			if (e.Success)
			{
				HoneywellDeviceHub.GetInstance().RfIdRdr = e.Reader;
				SetAntennaPower(2100, 2100);
				//HoneywellDeviceHub.GetInstance().RfIdRdr.SetAntennaPower(new Com.Honeywell.Rfidservice.Rfid.AntennaPower[0]);
				var antPower = e.Reader.GetAntennaPower();
				//Intent intent = new Intent(this, typeof(RfIdReaderActivity));
				//StartActivity(intent);
			}
		}

		private void SetAntennaPower(int readPower, int writePower)
		{
			Com.Honeywell.Rfidservice.Rfid.AntennaPower[] attenas = new Com.Honeywell.Rfidservice.Rfid.AntennaPower[1];
			Com.Honeywell.Rfidservice.Rfid.AntennaPower antenna = new Com.Honeywell.Rfidservice.Rfid.AntennaPower(1, readPower, writePower);
			attenas[0] = antenna;
			HoneywellDeviceHub.GetInstance().RfIdRdr.SetAntennaPower(attenas);
		}

		private void BtnBTScan_Click(object sender, EventArgs e)
		{

			if (_bluetoothAdapter != null)
				_bluetoothAdapter.StartDiscovery();

		}

		private void BtnInit_Click(object sender, EventArgs e)
		{

			if (btRfidScannerDevice == null) return;

			if (!IsConnected())
			{

				Disconnect();

				Connect(btRfidScannerDevice.Address);

				SetSelectedDev(btRfidScannerDevice);

			}

			ConnectionState connState = GetConnectionState();

			btnScanTags.Enabled = true;

		}

		private void BtnScanTags_Click(object sender, EventArgs e)
		{

			ConnectionState connState = GetConnectionState();
			if (connState == ConnectionState.StateConnected)
			{
				try
				{
					_rfidManager.CreateReader();
				}
				catch (Exception ex)
				{
					var testerr = ex.Message;
				}
			}
			else
			{
				Toast.MakeText(this, $"Connection state: {connState}", ToastLength.Short).Show();
			}

		}

	}

}
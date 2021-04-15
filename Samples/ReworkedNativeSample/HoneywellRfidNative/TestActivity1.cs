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
using System.Threading;

namespace HoneywellRfidNative
{

	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
	public class TestActivity1 : AppCompatActivity
	{

		static string mac = "0C:23:69:19:5E:3D";
		static BluetoothDevice btRfidScannerDevice = null;

		private Context _context;

		public class BluetoothDeviceReceiver : BroadcastReceiver
		{
			public override void OnReceive(Context context, Intent intent)
			{

				var action = intent.Action;

				// Get the device
				var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);

				if (action == BluetoothDevice.ActionFound && device != null)
				{
					if (device.Address == mac)
					{
						//if (device.BondState != Bond.Bonded)
						//{
						//	Console.WriteLine($"Found device with name: {device.Name} and MAC address: {device.Address}");
						//	device.CreateBond();
						//}
						btRfidScannerDevice = device;
					}
				}
			}
		}

		Button btnInit;
		Button btnScanTags;
		private BluetoothDeviceReceiver _receiver;
		private BluetoothAdapter _bluetoothAdapter = null;
		private RfidManager _rfidManager;

		private string[] _permissions = new string[]{Manifest.Permission.AccessCoarseLocation,
			Manifest.Permission.WriteExternalStorage};

		private List<string> _dismissPermissionList = new List<string>();

		private const int PERMISSION_REQUEST_CODE = 1;

		protected override void OnCreate(Bundle savedInstanceState)
		{

			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.testactivity1);

			btnInit = FindViewById<Button>(Resource.Id.btnInit);
			btnInit.Click += BtnInit_Click;

			btnScanTags = FindViewById<Button>(Resource.Id.btnScanTags);
			btnScanTags.Click += BtnScanTags_Click;

			Init();

			RequestDynamicPermissions();

			// Register for broadcasts when a device is discovered
			_receiver = new BluetoothDeviceReceiver();
			//RegisterReceiver(_receiver, new IntentFilter(BluetoothDevice.ActionFound, BluetoothDevice.ActionBondStateChanged));
			using (IntentFilter intentFilter = new IntentFilter(BluetoothDevice.ActionFound))
			{
				intentFilter.AddAction(BluetoothDevice.ActionBondStateChanged);
				RegisterReceiver(_receiver, intentFilter);
			}

			_bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
			if (_bluetoothAdapter != null)
				_bluetoothAdapter.StartDiscovery();

		}

		private void Init()
		{

			_context = this;
			
			_rfidManager = HoneywellDeviceHub.GetInstance().RfidMgr;

			SetSelectedDev(null);

			Disconnect();

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
			//btnScanTags.Enabled = false;
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

			//btnScanTags.Enabled = IsConnected();

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
				//_connectionHandler.SendEmptyMessage(BleMsgConsts.MSG_CREATE_READER_SUCCESSFULLY);
			}
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

			Thread.Sleep(2000);

			bool isConnected = IsConnected();
			if (isConnected)
			{
				_rfidManager.CreateReader();
			}

			//btnScanTags.Enabled = true;

		}

		private void BtnScanTags_Click(object sender, EventArgs e)
		{

			//if (IsConnected() && GetSelectedDev() != null)
			//{

				Intent intent = new Intent(this, typeof(RfIdReaderActivity));
				StartActivity(intent);

			//}

		}

	}

}
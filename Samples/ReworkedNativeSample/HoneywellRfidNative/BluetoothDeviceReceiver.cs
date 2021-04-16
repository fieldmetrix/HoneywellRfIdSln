using Android.Content;
using System;
using Android.Bluetooth;

namespace HoneywellRfidNative
{

	[BroadcastReceiver]
	public class BtBroadcastReceiver : BroadcastReceiver
	{

		public event EventHandler DeviceReceived;

		public override void OnReceive(Context context, Intent intent)
		{
			string action = intent.Action;

			if (BluetoothDevice.ActionFound == action)
			{
				BluetoothDevice deviceBt = intent.GetParcelableExtra(BluetoothDevice.ExtraDevice) as BluetoothDevice;

				if (deviceBt != null)
					DeviceReceived?.Invoke(deviceBt, new EventArgs());
			}
		}

	}

}
using System;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using HoneywellRfId.Events;

namespace HoneywellRfId.Broadcast
{


    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "android.bluetooth.adapter.action.STATE_CHANGED" })]
    public class BtReceiver : BroadcastReceiver
    {
        public event EventHandler<BtStateChangedEventArgs> OnBtStateChange;

        public override void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;

            if (BluetoothAdapter.ActionStateChanged.Equals(action))
            {
                var args = new BtStateChangedEventArgs(intent.GetIntExtra(BluetoothAdapter.ExtraState, 0));
                OnBtStateChange?.Invoke(this, args);
            }
        }
    }
}
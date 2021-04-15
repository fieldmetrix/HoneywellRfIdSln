using System;

namespace HoneywellRfidNative.Events
{
    public class BtStateChangedEventArgs : EventArgs
    {
        public Android.Bluetooth.State BtState { get; set; }

        public BtStateChangedEventArgs(int state)
        {
            BtState = (Android.Bluetooth.State)state;
        }
    }
}
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.OS;
using HoneywellRfId.Consts;
using HoneywellRfId.Handlers;

namespace HoneywellRfId.Callbacks
{
    public class BleScanCallBack : Java.Lang.Object, BluetoothAdapter.ILeScanCallback
    {
        private BtConnectionHandler _btConnectionHandler;

        public BleScanCallBack(BtConnectionHandler handler)
        {
            _btConnectionHandler = handler;
        }

        public void OnLeScan(BluetoothDevice device, int rssi, byte[] scanRecord)
        {
            if (device.Name != null)
            {
                Message msg = _btConnectionHandler.ObtainMessage(BleMsgConsts.MSG_ON_BLE_DEV_FOUND);
                msg.Arg1 = rssi;
                msg.Obj = device;
                _btConnectionHandler.SendMessage(msg);
            }
        }
    }
}
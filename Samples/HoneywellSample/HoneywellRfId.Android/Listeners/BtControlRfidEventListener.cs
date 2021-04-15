using Android.Content;
using Com.Honeywell.Rfidservice;
using Com.Honeywell.Rfidservice.Rfid;
using HoneywellRfId;
using HoneywellRfId.Consts;
using HoneywellRfId.Views;
using Java.Lang;

namespace HoneywellRfIdShared.Listeners
{
    public class BtControlRfidEventListener : Java.Lang.Object, IEventListener
    {
        private ReaderBtControl _control;
        private App _app;

        public BtControlRfidEventListener(App app, ReaderBtControl control)
        {
            _app = app;
            _control = control;
        }

        public void OnDeviceConnected(Object data)
        {
            _control.BleScanListAdapter.NotifyDataSetChanged();
            _control.ConnectionHandler.SendEmptyMessageDelayed(BleMsgConsts.MSG_DEALY_CREATE_READER, 1000);

        }

        public void OnDeviceDisconnected(Object data)
        {
            _control.AccessReadBtn.Enabled = false;
            _control.BleScanListAdapter.NotifyDataSetChanged();
            _control.ConnectionHandler.RemoveMessages(BleMsgConsts.MSG_DEALY_CREATE_READER);
        }

        public void OnReaderCreated(bool success, RfidReader reader)
        {
            if (success)
            {
                _app.RfIdRdr = reader;
                _control.ConnectionHandler.SendEmptyMessage(BleMsgConsts.MSG_CREATE_READER_SUCCESSFULLY);
                Intent intent = new Intent(_control, typeof(ReaderMain));
                _control.StartActivity(intent);
    } else {
                _control.ConnectionHandler.SendEmptyMessage(BleMsgConsts.MSG_CREATE_READER_FAILED);
            }
        }

        public void OnRfidTriggered(bool triggered)
        {
        }

        public void OnTriggerModeSwitched(TriggerMode mode)
        {
        }
    }
}
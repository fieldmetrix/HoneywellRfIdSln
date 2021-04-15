using Com.Honeywell.Rfidservice;
using Com.Honeywell.Rfidservice.Rfid;
using HoneywellRfId;
using HoneywellRfId.Fragments;
using Java.Lang;

namespace HoneywellRfIdShared.Listeners
{
    public class RfidEventListener : Java.Lang.Object, IEventListener
    {
        private ReaderInventoryFragment _control;
        private App _app;

        public RfidEventListener(App app, ReaderInventoryFragment _fragment)
        {
            _app = app;
            _control = _fragment;
        }

        public void OnDeviceConnected(Object data)
        {


        }

        public void OnDeviceDisconnected(Object data)
        {
            _control.IsReading = false;
            _control.IsReading_fast = false;
            _control.StopReadUI();
        }

        public void OnReaderCreated(bool success, RfidReader reader)
        {

        }

        public void OnRfidTriggered(bool triggered)
        {
            if (triggered)
            {
                /*
                 * 防止PDA上已经开始Inventory了，用户又在手柄上按了下trigger
                 * Fix QINGCS-21
                 * Press reader button after click start in fast mode, the speed isn't normal
                 */
                if (_control.IsReading_fast || _control.IsReading)
                {
                    _control.StopRead();
                }
                _control.BeginRead();
            }
            else
            {
                _control.StopRead();
            }
        }
    

        public void OnTriggerModeSwitched(TriggerMode mode)
        {
        }
    }
}
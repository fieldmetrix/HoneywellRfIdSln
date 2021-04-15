using Com.Honeywell.Rfidservice;
using Com.Honeywell.Rfidservice.Rfid;
using HoneywellRfIdShared.Messaging;
using HoneywellRfIdShared.Messaging.Messages;
using Java.Lang;

namespace HoneywellRfIdShared.Listeners
{
    public class RfidEventListener : Java.Lang.Object, IEventListener
    {
        public void OnDeviceConnected(Object data)
        {
            PubSubHandler.GetInstance().Publish(new RfidConnectedMessage(this, data));
        }

        public void OnDeviceDisconnected(Object data)
        {
            PubSubHandler.GetInstance().Publish(new RfidDisconnectedMessage(this, data));
        }

        public void OnReaderCreated(bool success, RfidReader reader)
        {
           
        }

        public void OnRfidTriggered(bool triggered)
        {
            PubSubHandler.GetInstance().Publish(new RfidTriggeredMessage(this, triggered));
        }

        public void OnTriggerModeSwitched(TriggerMode mode)
        {
            PubSubHandler.GetInstance().Publish(new RfidTriggerModeMessage(this, mode));
        }
    }
}
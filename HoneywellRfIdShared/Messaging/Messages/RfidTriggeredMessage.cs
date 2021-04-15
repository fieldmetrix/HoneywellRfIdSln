using Com.Honeywell.Rfidservice.Rfid;
using Java.Lang;
using Messenger;

namespace HoneywellRfIdShared.Messaging.Messages
{
    public class RfidTriggeredMessage : MessengerMessage
    {
        public bool Triggered { get; set; }

        public RfidTriggeredMessage(object sender, bool triggered) : base(sender)
        {
            Triggered = triggered;
        }
    }
}
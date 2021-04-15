using Com.Honeywell.Rfidservice;
using Java.Lang;
using Messenger;

namespace HoneywellRfIdShared.Messaging.Messages
{
    public class RfidTriggerModeMessage : MessengerMessage
    {
        public TriggerMode Mode { get; set; }

        public RfidTriggerModeMessage(object sender, TriggerMode mode) : base(sender)
        {
            Mode = mode;
        }
    }
}
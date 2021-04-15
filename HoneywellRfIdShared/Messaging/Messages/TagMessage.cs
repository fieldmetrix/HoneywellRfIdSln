using Com.Honeywell.Rfidservice.Rfid;
using Messenger;

namespace HoneywellRfIdShared.Messaging.Messages
{
    public class TagMessage : MessengerMessage
    {
        public TagReadData[] TagReadDataReading { get; set; }

        public TagMessage(object sender, TagReadData[] _tagReadDataReading) : base(sender)
        {
            TagReadDataReading = _tagReadDataReading;
        }
    }
}
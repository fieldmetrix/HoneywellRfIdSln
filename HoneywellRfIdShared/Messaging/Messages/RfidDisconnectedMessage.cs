using Java.Lang;
using Messenger;

namespace HoneywellRfIdShared.Messaging.Messages
{
    public class RfidDisconnectedMessage : MessengerMessage
    {
        public Object Data { get; set; }

        public RfidDisconnectedMessage(object sender, Object data) : base(sender)
        {
            Data = data;
        }
    }
}
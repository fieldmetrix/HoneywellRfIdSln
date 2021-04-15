using Java.Lang;
using Messenger;

namespace HoneywellRfIdShared.Messaging.Messages
{
    public class RfidConnectedMessage : MessengerMessage
    {
        public Object Data { get; set; }

        public RfidConnectedMessage(object sender, Object data) : base(sender)
        {
            Data = data;
        }
    }
}
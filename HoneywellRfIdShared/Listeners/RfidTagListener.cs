using Com.Honeywell.Rfidservice;
using Com.Honeywell.Rfidservice.Rfid;
using HoneywellRfIdShared.Messaging;
using HoneywellRfIdShared.Messaging.Messages;
using Java.Lang;

namespace HoneywellRfIdShared.Listeners
{
    public class RfidTagListener : Java.Lang.Object, IOnTagReadListener
    {
        public void OnTagRead(TagReadData[] tagReadData)
        {
            PubSubHandler.GetInstance().Publish(new TagMessage(this, tagReadData));
        }
    }
}
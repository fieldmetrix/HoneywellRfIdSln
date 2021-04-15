using Com.Honeywell.Rfidservice.Rfid;
using HoneywellRfId.Fragments;

namespace HoneywellRfIdShared.Listeners
{
    public class RfidTagListener : Java.Lang.Object, IOnTagReadListener
    {
        private ReaderInventoryFragment _fragment;

        public RfidTagListener (ReaderInventoryFragment fragment)
        {
            _fragment = fragment;
        }

        public void OnTagRead(TagReadData[] tagReadData)
        {
            _fragment.TotalNum += tagReadData.Length;
            _fragment.UpdateList(tagReadData);
        }
    }
}
using System;
using Android.OS;
using HoneywellRfId.Views;

namespace HoneywellRfId.Handlers
{
    public class BtConnectionHandler : Handler
    { 
        private WeakReference<ReaderBtControl> _ref;

        public BtConnectionHandler(ReaderBtControl act)
        {
            _ref = new WeakReference<ReaderBtControl>(act);
        }

        public override void HandleMessage(Message msg)
        {
            base.HandleMessage(msg);
            ReaderBtControl target;
            _ref.TryGetTarget(out target);

            if (target != null)
            {
                target.HandleMessage(msg);
            }
        }
    }
}
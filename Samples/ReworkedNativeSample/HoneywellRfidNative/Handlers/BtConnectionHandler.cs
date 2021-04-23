using System;
using Android.OS;

namespace HoneywellRfidNative.Handlers
{
    public class BtConnectionHandler : Handler
    {
        private WeakReference<MainActivity> _ref;

        public BtConnectionHandler(MainActivity act)
        {
            _ref = new WeakReference<MainActivity>(act);
        }

        public override void HandleMessage(Message msg)
        {
            base.HandleMessage(msg);
            MainActivity target;
            _ref.TryGetTarget(out target);

            if (target != null)
            {
                target.HandleMessage(msg);
            }
        }
    }
}
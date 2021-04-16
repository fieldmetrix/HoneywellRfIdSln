using System;
using Android.OS;

namespace HoneywellRfidNative.Handlers
{
    public class BtConnectionHandler : Handler
    {
        private WeakReference<TestActivity2> _ref;

        public BtConnectionHandler(TestActivity2 act)
        {
            _ref = new WeakReference<TestActivity2>(act);
        }

        public override void HandleMessage(Message msg)
        {
            base.HandleMessage(msg);
            TestActivity2 target;
            _ref.TryGetTarget(out target);

            if (target != null)
            {
                target.HandleMessage(msg);
            }
        }
    }
}
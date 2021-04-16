using System;
using Android.OS;

namespace HoneywellRfidNative.Handlers
{
    public class BtConnectionHandler : Handler
    {
        private WeakReference<TestActivity1> _ref;

        public BtConnectionHandler(TestActivity1 act)
        {
            _ref = new WeakReference<TestActivity1>(act);
        }

        public override void HandleMessage(Message msg)
        {
            base.HandleMessage(msg);
            TestActivity1 target;
            _ref.TryGetTarget(out target);

            if (target != null)
            {
                target.HandleMessage(msg);
            }
        }
    }
}
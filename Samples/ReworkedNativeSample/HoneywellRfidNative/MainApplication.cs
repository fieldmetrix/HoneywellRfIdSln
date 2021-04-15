using System;
using Android.App;
using Android.Runtime;
using HoneywellRfIdShared.Hub;

namespace HoneywellRfidNative
{
#if DEBUG
    [Application(Debuggable = true)]
#else
        [Application(Debuggable = false)]
#endif
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
           
            HoneywellDeviceHub.Create(this);
        }
    }
}
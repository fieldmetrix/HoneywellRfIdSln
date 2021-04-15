using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HoneywellRfId.Consts;
using HoneywellRfId.Views;

namespace HoneywellRfId.Handlers
{
    public class ReaderHandler : Handler
    {
        private WeakReference<ReaderMain> _ref;

        public ReaderHandler(ReaderMain act)
        {
            _ref = new WeakReference<ReaderMain>(act);
        }

        public override void HandleMessage(Message msg)
        {
            base.HandleMessage(msg);

            ReaderMain target;
            _ref.TryGetTarget(out target);

            ReaderMain act = target;

            if (act == null)
            {
                return;
            }

            switch (msg.What)
            {
                case ReaderConsts.MSG_LOOP_TEMP:
                    App app = (App)act.Application;
                    if (app.CheckIsRFIDReady())
                    {
                        float t = app.RfidMgr.BatteryTemperature;
                        int chargeCycle = app.RfidMgr.BatteryChargeCycle;
                        app.BatteryTemperature = t;
                        app.BatteryChargeCycle = chargeCycle;

                        if (chargeCycle > 500 && !act.BatteryReminder)
                        {
                            act.BatteryReminder = true;
                            Toast.MakeText(act, Resource.String.toast_battery_charge_cycle_reminder, ToastLength.Long).Show();
                        }
                    }

                    act.StartLoopTemperatureTimer();
                    break;
            }
        }
    }
}
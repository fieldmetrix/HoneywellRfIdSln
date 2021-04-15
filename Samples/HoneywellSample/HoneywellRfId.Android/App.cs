using System;
using System.Collections.Generic;
using Android.App;
using Android.Bluetooth;
using Android.Runtime;
using Android.Widget;
using Com.Honeywell.Rfidservice;
using Com.Honeywell.Rfidservice.Rfid;

namespace HoneywellRfId
{
    [Application]
    public class App : Application
    {
        public string[] Coname;
        public string[] tagListTitles;
        private RfidReader _rfidReader;

        // public SharedPrefManager mSharedPrefManager;

        //BT control
        public List<BluetoothDevice> _bleScanDevices = new List<BluetoothDevice>();
        public Dictionary<string, int> _bleScanDevicesRssi = new Dictionary<string, int>();
        public BluetoothDevice _selectedBleDev;

        public List<Dictionary<string, string>> ListMs =  new List<Dictionary<string, string>>();

        //Write Tag Fragment
        public string SelectedEpc;

        //Locate Tag
        public bool IsFindingGood = false;

        public float _batteryTemperature = 0;
        public int _batteryChargeCycle = 0;

        private static App _instance;
        private RfidManager _rfidMgr;
        //    public boolean enableRfidFwUpdate;
        //     public boolean debugMode;

        public RfidManager RfidMgr
        {
            get
            {
                return _rfidMgr;
            }
        }

        public RfidReader RfIdRdr
        {
            get
            {
                return _rfidReader;
            }

            set
            {
                _rfidReader = value;
            }
        }

        public Dictionary<string, int> BleScanDevicesRssi
        {
            get
            {
                return _bleScanDevicesRssi;
            }
        }

        public BluetoothDevice SelectedBleDev
        {
            get
            {
                return _selectedBleDev;
            }

            set
            {
                _selectedBleDev = value;
            }
        }

        public float BatteryTemperature
        {
            get
            {
                return _batteryTemperature;
            }

            set
            {
                _batteryTemperature = value;
            }
        }

        public int BatteryChargeCycle
        {
            get
            {
                return _batteryChargeCycle;
            }

            set
            {
                _batteryChargeCycle = value;
            }
        }

        public List<BluetoothDevice> BleScanDevices
        {
            get
            {
                return _bleScanDevices;
            }
        }

        public static App GetInstance()
        {
            return _instance;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            _instance = this;

            _rfidMgr = RfidManager.GetInstance(this);

            Coname = Resources.GetStringArray(Resource.Array.coname_texts);
            tagListTitles = Resources.GetStringArray(Resource.Array.coname_texts);
            //  LogUtils.e("App onCreate");
            //    setListTitle();
            //        LogUtils.e(BuildConfig.UI_STYLE + ":" + BuildConfig.FLAVOR);
            //          mSharedPrefManager = SharedPrefManager.getInstance(this);

            /*new Handler().post(new Runnable() {
                        @Override
                        public void run()
                        {
                            mSharedPrefManager.loadDefaultSettings();
                            boolean debug = mSharedPrefManager.getBoolean(getString(R.string.debug_mode), false);
                            setDebugMode(debug);
                        }
                    });*/
        }

        public App(IntPtr handle, JniHandleOwnership ownerShip) : base(handle, ownerShip)
        {
        }

        public bool IsRFIDReady()
        {
            return _rfidMgr.ReaderAvailable() && _rfidMgr.ConnectionState == ConnectionState.StateConnected && _rfidMgr.TriggerMode == TriggerMode.Rfid;
        }

        public bool IsReady()
        {
            return _rfidMgr.ReaderAvailable() && _rfidMgr.ConnectionState == ConnectionState.StateConnected;
        }

        public bool IsBatteryTemperatureTooHigh()
        {
            return _batteryTemperature >= 60;
        }

        public bool CheckIsRFIDReady()
        {
            if (_rfidMgr.ConnectionState != ConnectionState.StateConnected)
            {
                Toast.MakeText(this, GetString(Resource.String.toast_error1), ToastLength.Short).Show();
                return false;
            }
            if (!_rfidMgr.ReaderAvailable())
            {
                Toast.MakeText(this, GetString(Resource.String.toast_error2), ToastLength.Short).Show();
                return false;
            }
            if (_rfidMgr.TriggerMode != TriggerMode.Rfid)
            {
                Toast.MakeText(this, GetString(Resource.String.toast_error3), ToastLength.Short).Show();
                return false;
            }
            return true;
        }

        public bool CheckIsReady()
        {
            if (_rfidMgr.ConnectionState != ConnectionState.StateConnected)
            {
                Toast.MakeText(this, GetString(Resource.String.toast_error1), ToastLength.Short).Show();
                return false;
            }
            if (!_rfidMgr.ReaderAvailable())
            {
                Toast.MakeText(this, GetString(Resource.String.toast_error2), ToastLength.Short).Show();
                return false;
            }
            return true;
        }
    }
}
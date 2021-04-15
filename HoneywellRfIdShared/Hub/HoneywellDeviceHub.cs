using Android.Bluetooth;
using Android.Content;
using Com.Honeywell.Rfidservice;
using Com.Honeywell.Rfidservice.Rfid;
using HoneywellRfIdShared.Listeners;
using System.Collections.Generic;

namespace HoneywellRfIdShared.Hub
{
    public class HoneywellDeviceHub
    {
        private RfidReader _rfidReader;
        private static HoneywellDeviceHub _instance;
        private Context _context;
        private RfidManager _rfidMgr;
        private BluetoothDevice _selectedBleDev;

        private Dictionary<string, int> _bleScanDevicesRssi = new Dictionary<string, int>(0);

        private RfidEventListener _rfidEventListener;

        private RfidTagListener _tagReaderListener;

        public bool IsReading { get; set; }

        public Dictionary<string, int> BleScanDevicesRssi
        {
            get
            {
                return _bleScanDevicesRssi;
            }

            set
            {
                _bleScanDevicesRssi = value;
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

        public static HoneywellDeviceHub GetInstance()
        {
            return _instance;
        }

        public HoneywellDeviceHub(Context context)
        {
            _context = context;
            _rfidMgr = RfidManager.GetInstance(context);

            _rfidEventListener = new RfidEventListener();
            _tagReaderListener = new RfidTagListener();
        }

        public static void Create(Context context)
        {
            _instance = new HoneywellDeviceHub(context);
           
        }

        public bool CheckIsRFIDReady()
        {
            if (_rfidMgr.ConnectionState != ConnectionState.StateConnected)
            {
                return false;
            }
            if (!_rfidMgr.ReaderAvailable())
            {
                return false;
            }

            if (_rfidMgr.TriggerMode != TriggerMode.Rfid)
            {
                return false;
            }
            return true;
        }

        public bool CheckIsReady()
        {
            if (_rfidMgr.ConnectionState != ConnectionState.StateConnected)
            {
                return false;
            }
            if (!_rfidMgr.ReaderAvailable())
            {
                return false;
            }
            return true;
        }

        private TagReadOption SetupTagReadOptions()
        {
            var options = new TagReadOption
            {
                ReadCount = true,
                Rssi = false,
                AntennaId = false,
                Frequency = false,
                Timestamp = false,
                Protocol = false,
                Data = false,
                StopPercent = 0
            };

            return options;
        }


        public void StartReading()
        {
            if (IsReading)
                StopReading();

            var options = SetupTagReadOptions();
            _rfidReader.SetOnTagReadListener(_tagReaderListener);
            _rfidReader.Read(TagAdditionData.None, options);


            IsReading = true;
        }

        public void StopReading()
        {
            _rfidReader.RemoveOnTagReadListener(_tagReaderListener);
            IsReading = false;
        }


        public void Attach()
        {
            _rfidMgr.AddEventListener(_rfidEventListener);
        }

        public void Detach()
        {
            if (IsReading)
                StopReading();


            _rfidMgr.RemoveEventListener(_rfidEventListener);
        }

       
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Media;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Com.Honeywell.Rfidservice;
using Com.Honeywell.Rfidservice.Rfid;
using HoneywellRfId.Adapters;
using HoneywellRfId.Callbacks;
using HoneywellRfId.Interfaces;
using HoneywellRfId.Utils;
using HoneywellRfIdShared.Listeners;
using Xamarin.Essentials;

namespace HoneywellRfId.Fragments
{
    public class ReaderInventoryFragment : BaseFragment, View.IOnClickListener
    {
        private bool _isResumed;
        public bool IsReading = false;
        public bool IsReading_fast = false;
        private bool _isShow = true;
        private IOnFragmentInteractionListener mListener;
        private SoundPool _soundPool;
        private int _soundId;
        private App _app;

        private NormalModeDashBoard _normalModeInfo = new NormalModeDashBoard();

        private long ReadBeginTime = 0;

        private RecyclerView _recyclerView;
        private ReaderReadAdapter _readerReadAdapter;
        private View _view;
        private Toolbar _toolbar;
        private RelativeLayout _animLayout;
        private TextView _rssiTv;
        private ProgressBar _rssiProgressBar;
        private TextView _onceCntTv, _onceNumTv, _onceTimeTv, _totalCntTv, _totalNumTv, _totalTimeTv, _averageSpeedTv;
        private TextView _onceCntTitleTv;
        private Button _beginReadSingleBtn, _stopReadSingleBtn;

        private HandlerThread _readHandlerThread = new HandlerThread("ReadHandler");
        private Handler _readHandler;
        private Handler _uiHandler;

        private const int MSG_UPDATE_UI_NORMAL_MODE = 0;
        private const int MSG_UPDATE_UI_FAST_MODE = 1;
        private const int FAST_MODE_UPDATE_UI_GAP = 200;

        public long mReadBeginTime = 0;
        public int TotalNum; //总标签次数
        public int TotalNum_previous = 0;
        public int FastScanUpdateUICnts = 0;
        public long TimeRecordPerAboutSecond = 0;

        private Dictionary<string, TagInfo> TagMap = new Dictionary<string, TagInfo>();
        private TagReadOption TagReadOption = new TagReadOption();

        private IOnTagReadListener _dataListener;
        private IEventListener _rfidEventListener;

        public List<Dictionary<string, string>> OldList = new List<Dictionary<string, string>>();


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container,
                            Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            _view = inflater.Inflate(Resource.Layout.fragment_reader_inventory, container, false);
            //  InitToolBar();
            Init(_view);
            return _view;
        }

        private void InitToolBar()
        {
            _toolbar = (Toolbar)_view.FindViewById(Resource.Id.toolbar);
            _toolbar.Title = GetString(Resource.String.inventory_tag_text);
            _toolbar.SetNavigationIcon(Resource.Drawable.position_left);
            _toolbar.NavigationOnClick += _toolbar_NavigationOnClick;

        }

        private bool IsFastMode()
        {
            return false ;
        }

        private void _toolbar_NavigationOnClick(object sender, EventArgs e)
        {
            if (Activity != null)
            {
                Activity.Finish();
            }
        }

        private void Init(View parent)
        {
            _rfidEventListener = new RfidEventListener(_app,this);
            _dataListener = new RfidTagListener(this);

            _app = App.GetInstance();

            //InitHandler();
            _onceCntTitleTv = (TextView)parent.FindViewById(Resource.Id.once_cnt_title);
            _onceCntTv = (TextView)parent.FindViewById(Resource.Id.once_cnt_result);
            _onceNumTv = (TextView)parent.FindViewById(Resource.Id.once_num_result);
            _onceTimeTv = (TextView)parent.FindViewById(Resource.Id.once_time_result);
            _totalCntTv = (TextView)parent.FindViewById(Resource.Id.total_cnt_result);
            _totalNumTv = (TextView)parent.FindViewById(Resource.Id.total_num_result);
            _totalTimeTv = (TextView)parent.FindViewById(Resource.Id.total_time_result);
            _averageSpeedTv = (TextView)parent.FindViewById(Resource.Id.average_speed_result);
            _beginReadSingleBtn = (Button)parent.FindViewById(Resource.Id.begin_read_single);
            _beginReadSingleBtn.SetOnClickListener(this);
            _stopReadSingleBtn = (Button)parent.FindViewById(Resource.Id.stop_read_single);
            _stopReadSingleBtn.SetOnClickListener(this);
            _rssiProgressBar = (ProgressBar)  parent.FindViewById(Resource.Id.finding_good_progressbar);
            _animLayout = (RelativeLayout)parent.FindViewById(Resource.Id.frame_anim_viewgroup);
            var dm = Resources.DisplayMetrics;
            int screenWidth = dm.WidthPixels;
            ViewGroup.LayoutParams layoutParams = _animLayout.LayoutParameters;
            layoutParams.Width = screenWidth;

            _rssiTv = (TextView)parent.FindViewById(Resource.Id.tv_rssi);

            _recyclerView = (RecyclerView)parent.FindViewById(Resource.Id.recycler_view);
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(Activity) {
             //@Override
             //public boolean canScrollHorizontally()
             //{
             //    return false;
             //}
         }; 

         linearLayoutManager.Orientation = LinearLayoutManager.Vertical;
        
          _recyclerView.SetLayoutManager(linearLayoutManager);
         _readerReadAdapter = new ReaderReadAdapter(_app, _app.ListMs);
         _readerReadAdapter.UpdateShowItems();
         _recyclerView.SetAdapter(_readerReadAdapter);
         _recyclerView.SetItemAnimator(null);
         _recyclerView.HasFixedSize = false;
        }

        private void UpdateDashBoard()
        {
            

            MainThread.BeginInvokeOnMainThread(() => {

                NormalModeDashBoard info = _normalModeInfo;
                _onceCntTv.Text = info.OnceCount + "";
                _onceNumTv.Text = info.OnceNum + "";
                _onceTimeTv.Text = info.OnceTime + "ms";

                if (info.TotalTime > 500)
                {
                    _totalCntTv.Text = info.TotalCount + "";
                    _totalNumTv.Text = TotalNum + "";
                    _totalTimeTv.Text = info.TotalTime + "ms";
                    _averageSpeedTv.Text = info.NumPerSecond + "pcs/s";
                }

            });

           
        }

        public static ReaderInventoryFragment NewInstance()
        {
            return new ReaderInventoryFragment();
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.begin_read_single:
                    BeginRead();
                    break;
                case Resource.Id.stop_read_single:
                    StopRead();
                    break;
                default:
                    break;
            }
        }

        public void BeginRead()
        {
            if (_app.IsBatteryTemperatureTooHigh())
            {
                BaseActivity act = (BaseActivity)Activity;

                if (act != null)
                {
                    act.ShowToast(Resource.String.toast_temp_too_high);
                }

                return;
            }

            try
            {
                if (_app.CheckIsRFIDReady())
                {
                    BeginReadUI();
                    ClearDataAndResetParams();

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        BeginReadInternal();

                    });
                }
            }
            catch (Exception e)
            {
            }
        }

        public void StopRead()
        {
            try
            {
                StopReadUI();
                if (_app.IsRFIDReady())
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        StopReadInternal();
                    });

                }
            }
            catch (Exception e)
            {
            }
        }

        public void StopReadUI()
        {
            _beginReadSingleBtn.Visibility = ViewStates.Visible;
            _stopReadSingleBtn.Visibility = ViewStates.Visible;
            _animLayout.Visibility = ViewStates.Gone;
        }

        private void StopReadInternal()
        {
            if (!IsFastMode())
            {
                IsReading = false;
            }
            else
            {
                IsReading_fast = false;
                RfidReader reader = GetReader();
                reader.StopRead();
                reader.RemoveOnTagReadListener(_dataListener);
            }
            _app.RfidMgr.SetLEDBlink(false);
            StopHandleBeeper();
        }

        private void StopHandleBeeper()
        {
            _app.RfidMgr.SetBeeper(false, 0x05, 0x05);
        }

        public void BeginReadUI()
        {
            _beginReadSingleBtn.Visibility = ViewStates.Gone;
            _stopReadSingleBtn.Visibility = ViewStates.Visible;

            if (!IsFastMode())
            {
                _onceCntTitleTv.Text = "Once cnt";
            }
            else
            {
                _onceCntTitleTv.Text = "Real speed(pcs/s)";
            }

            _onceCntTv.Text = "0";
            _onceNumTv.Text = "0";
            _onceTimeTv.Text = "0ms";
            _totalCntTv.Text = "0";
            _totalNumTv.Text = "0";
            _totalTimeTv.Text = "0ms";
            _averageSpeedTv.Text = "0pcs/s";

            if (_app.IsFindingGood)
            {
                _animLayout.Visibility = ViewStates.Visible;
            }
        }

        private RfidReader GetReader()
        {
            return _app.RfIdRdr;
        }


        private void BeginReadInternal()
        {
            _app.RfidMgr.SetLEDBlink(true);

           
                IsReading_fast = true;

                SetQuickModeParams();

                string additionDataType = "None";
                RfidReader reader = GetReader();
                reader.SetOnTagReadListener(_dataListener);
                reader.Read(TagAdditionData.Get(additionDataType), TagReadOption);


                Android.OS.Message msg = Android.OS.Message.Obtain();
                msg.What = MSG_UPDATE_UI_FAST_MODE;
                _uiHandler.SendMessage(msg);
        }

        public void UpdateList(TagReadData[] trds)
        {
            int onceNums = 0;

            NormalModeDashBoard info = _normalModeInfo;
            long start = DateTime.Now.Millisecond;

            long cur = DateTime.Now.Millisecond;
            info.OnceTime = cur - start;
            info.TotalTime = cur - mReadBeginTime;

            

            foreach (TagReadData trd in trds)
            {
                onceNums += trd.ReadCount;
                UpdateList(trd);
            }

            TotalNum = onceNums;
            info.OnceNum = onceNums;
            info.NumPerSecond = (int)(TotalNum * 1000 / info.TotalTime);
            info.OnceCount = trds.Length;
            info.TotalCount = TagMap.Count;

            UpdateDashBoard();
            UpdateRecyclerView();

        }

        public void UpdateList(TagReadData trd)
        {
            string[] keys = _app.Coname;
            int aDataLen = trd.GetAdditionData() != null ? trd.GetAdditionData().Length : 0;
            string epc = trd.EpcHexStr;

            if (!TagMap.ContainsKey(epc))
            {
                TagInfo ati = new TagInfo(trd);

                if (_app.IsFindingGood)
                {
                    if (epc.Equals(_app.SelectedEpc))
                    {
                        TagMap[epc] = ati;
                    }
                }
                else
                {
                    TagMap[epc] = ati;
                }

                // list
                Dictionary<string, string> m = new Dictionary<string, string>
                {
                    ["ecp_org"] = epc,
                    [keys[0]] = TagMap.Count.ToString(),
                    [keys[1]] = epc.Length < 24 ? string.Format("%-24s", epc) : epc,
                    [keys[2]] = trd.ReadCount.ToString(),
                    [keys[3]] = trd.Antenna.ToString(),
                    [keys[4]] = "gen2",
                    [keys[5]] = trd.Rssi.ToString(),
                    [keys[6]] = trd.Frequency.ToString(),
                    [keys[8]] = trd.Time.ToString()
                };

                if (aDataLen > 0)
                {
                    m[keys[7]] = StrUtil.ToHexString(trd.GetAdditionData(), 0, aDataLen);
                }
                else
                {
                    m[keys[7]] = "                 ";
                }

                if (_app.IsFindingGood)
                {
                    if (_app.SelectedEpc.Equals(m[keys[1]]))
                    {
                        _app.ListMs.Add(m);
                    }
                }
                else
                {
                    _app.ListMs.Add(m);
                }
            }
            else
            {
                TagInfo atf = TagMap[trd.EpcHexStr];
                atf.ReadCount += trd.ReadCount;

                for (int k = 0; k < _app.ListMs.Count; k++)
                {

                    Dictionary<string, string> m = (Dictionary<string, string>)_app.ListMs[k];

                    //if (epc.Equals(m["ecp_org"]))
                    
                    if (m.ContainsKey(epc))
                    {
                        m[keys[2]] = atf.ReadCount.ToString();
                        m[keys[5]] = trd.Rssi.ToString();
                        m[keys[6]] = trd.Frequency.ToString();

                        if (aDataLen > 0)
                        {
                            m[keys[7]] = StrUtil.ToHexString(trd.GetAdditionData(), 0, aDataLen);
                        }

                    }
                }
            }
        }

        private void ClearDataAndResetParams()
        {
            TagMap.Clear();
            _app.ListMs.Clear();
            ReadBeginTime = DateTime.Now.Millisecond;
            TotalNum = 0;
            TotalNum_previous = 0;
            FastScanUpdateUICnts = 0;
            TimeRecordPerAboutSecond = 0;
            AddListTitle();
            UpdateRecyclerView();
        }

        private void AddListTitle()
        {
            if (_app.ListMs.Count == 0)
            {
                Dictionary<string, string> h = new Dictionary<string,string>();
                String[] Coname = _app.Coname;
                for (int i = 0; i < Coname.Length; i++)
                {
                    h[Coname[i]] = _app.tagListTitles[i];
                }
                _app.ListMs.Add(h);
            }
        }

        private void UpdateRecyclerView()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {

                if (OldList != null)
                {
                    DiffUtil.DiffResult diffResult = DiffUtil.CalculateDiff(new DiffCallBack(_app, OldList, _app.ListMs), true);
                    diffResult.DispatchUpdatesTo(_readerReadAdapter);
                }
                OldList.Clear();
                for (int i = 0; i < _app.ListMs.Count; i++)
                {
                    Dictionary<string, string> m = (Dictionary<string, string>)((Dictionary<string, string>)_app.ListMs[i]);
                    OldList.Add(m);
                }

            });

        }


        private void SetQuickModeParams()
        {
            TagReadOption.ReadCount = false;
            TagReadOption.Rssi = false; 
            TagReadOption.AntennaId = false; 
            TagReadOption.Frequency = false;  
            TagReadOption.Timestamp = false; 
            TagReadOption.Protocol = false; 
            TagReadOption.Data = false;
            TagReadOption.StopPercent = 0;
        }


        public override void OnResume()
        {
            base.OnResume();
            // IsResumed = true;
            _app.RfidMgr.AddEventListener(_rfidEventListener);
        }

        public override void OnPause()
        {
            base.OnPause();
            // IsResumed = true;
            _app.RfidMgr.RemoveEventListener(_rfidEventListener);
        }

    }

    public class NormalModeDashBoard
    {
        public int OnceCount; //单次盘存标签个数
        public int OnceNum; //单次盘存标签次数
        public int TotalCount; //总标签个数
        public long OnceTime; //单次盘存用时
        public long TotalTime; //总用时，单位为ms
        public int NumPerSecond; //每秒多少次,平均值算出来的
    }


    public class TagInfo
    {
        public TagReadData d;
        public int ReadCount;

        public TagInfo(TagReadData trd)
        {
            d = trd;
        }
    }

}
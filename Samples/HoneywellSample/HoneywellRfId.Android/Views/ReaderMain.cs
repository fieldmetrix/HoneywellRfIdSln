using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using Google.Android.Material.BottomNavigation;
using HoneywellRfId.Consts;
using HoneywellRfId.Fragments;
using HoneywellRfId.Handlers;
using HoneywellRfId.Interfaces;
using System;

namespace HoneywellRfId.Views
{
    [Activity(Label = "ReaderMain")]
    public class ReaderMain : BaseActivity, BottomNavigationView.IOnNavigationItemSelectedListener, IOnFragmentInteractionListener
    {
        private bool _batteryReminder;
        private ReaderHandler _basicHandler;
        private int _selectedIndex;
        private AndroidX.Fragment.App.FragmentManager _fragmentManager;
        private BottomNavigationView _bottomNavigationView;

        private ReaderInventoryFragment _readerReadFrag;


        public bool BatteryReminder
        {
            get { return _batteryReminder; }
            set { _batteryReminder = value; }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _basicHandler = new ReaderHandler(this);
            StartLoopTemperatureTimer();
            SetContentView(Resource.Layout.reader_main);
            _selectedIndex = Intent.GetIntExtra("index", 0);
            Init(_selectedIndex);
        }

        private void Init(int index)
        {
            _fragmentManager = SupportFragmentManager;
            _bottomNavigationView = (BottomNavigationView) FindViewById(Resource.Id.navigation);
            _bottomNavigationView.SetOnNavigationItemSelectedListener(this);
            _bottomNavigationView.SelectedItemId = _bottomNavigationView.Menu.GetItem(index).ItemId;
        }

        public void StartLoopTemperatureTimer()
        {
            _basicHandler.SendEmptyMessageDelayed(ReaderConsts.MSG_LOOP_TEMP, 60 * 1000);
        }

        private void LoadFragment(int index)
        {
            var fragmentTransaction = _fragmentManager.BeginTransaction();
            HideAllTabs(fragmentTransaction);
            
            switch (index)
            {
                case 0:
                    if (_readerReadFrag == null)
                    {
                        _readerReadFrag = ReaderInventoryFragment.NewInstance();
                        fragmentTransaction.Add(Resource.Id.fragment_frame, _readerReadFrag);
                    }
                    else
                    {
                        fragmentTransaction.Show(_readerReadFrag);
                    }
                    break;
            }
            fragmentTransaction.Commit();
        }

        private void HideAllTabs(AndroidX.Fragment.App.FragmentTransaction fragTran)
        {
            if (_readerReadFrag != null)
            {
                fragTran.Hide(_readerReadFrag);
            }
        }


        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int lastSelectedItemId = _bottomNavigationView.SelectedItemId;
            int itemId = item.ItemId;
            switch (itemId)
            {
                case Resource.Id.reader_read_item:
                    LoadFragment(0);
                    break;
            }

            return true;
        }


        public void OnFragmentInteraction(Uri uri)
        {
          
        }
    }
}
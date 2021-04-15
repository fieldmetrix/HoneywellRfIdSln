using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using HoneywellRfId.Adapters;
using HoneywellRfId.Dto;
using System.Collections.Generic;
using Android.Content;
using HoneywellRfId.Views;
using Com.Honeywell.Rfidservice;
using Android;
using AndroidX.Core.Content;
using AndroidX.Core.App;

namespace HoneywellRfId
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : BaseActivity, GridView.IOnItemClickListener
    {
        private List<FunctionItem> _list = new List<FunctionItem>();
        private GridView _gridView;
        private CustomAdapter _adapter;
        private App _app;
        private long _exitTime;

        private const int PERMISSION_REQUEST_CODE = 1;

        private string[] _permissions = new string[]{Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.WriteExternalStorage};

        private List<string> _dismissPermissionList = new List<string>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource

            //     View decorView = Window.DecorView;
            //     decorView.SystemUiVisibility = View.SystemUiFlagLayoutFullscreen | View.SystemUiFlagLayoutStable | View.SystemUiFlagLightStatusBar;
            Window.SetStatusBarColor(Android.Graphics.Color.Transparent);

            SetContentView(Resource.Layout.activity_main);
            //getSupportActionBar().hide();
            //            initToolBar();
            Init();
            RequestDynamicPermissions();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            Intent intent = null;
            
            switch (position)
            {
                case 0:
                    intent = new Intent(this, typeof(ReaderBtControl));
                break;
            case 1:
                if (CheckBTIsConnected()) {

                }
                intent = new Intent(this, typeof(ReaderMain));
                intent.PutExtra("index", 0);
                break;
            case 2:
                if (CheckBTIsConnected()) {

                }
                intent = new Intent(this, typeof(ReaderMain));
                intent.PutExtra("index", 1);
                break;
            case 3:
                if (CheckBTIsConnected()) {

                }
                intent = new Intent(this, typeof(ReaderMain));
                intent.PutExtra("index", 2);
                break;
            case 4:
                if (CheckBTIsConnected()) {

                }
                intent = new Intent(this, typeof(ReaderMain));
                intent.PutExtra("index", 3);
                break;
            case 5:
                intent = new Intent(this, typeof(ReaderAbout));
                break;
            default:
                break;
        }
        if (intent != null) {
            StartActivity(intent);
}
        }

        private bool CheckBTIsConnected()
        {
            if (_app.RfidMgr.ConnectionState != ConnectionState.StateConnected)
            {
                Toast.MakeText(this, GetString(Resource.String.bluetooth_not_connected), ToastLength.Short).Show();
                return false;
            }
            return true;
        }

        private void RequestDynamicPermissions()
        {
                for (int i = 0; i < _permissions.Length; i++)
                {
                    if (ContextCompat.CheckSelfPermission(this, _permissions[i]) !=
                            Android.Content.PM.Permission.Granted)
                    {
                    _dismissPermissionList.Add(_permissions[i]);
                    }
                }
                if (_dismissPermissionList.Count > 0)
                {
                    ActivityCompat.RequestPermissions(this, _permissions, PERMISSION_REQUEST_CODE);
                }
        }

        private void Init()
        {
            _app = (App) Application;
            _gridView = (GridView) FindViewById(Resource.Id.functionset_grid);
           
         
            string[] convertTexts = Resources.GetStringArray(
                    Resource.Array.function_texts);
            var typedArray = Resources.ObtainTypedArray(
                    Resource.Array.function_icons);
            for (int index = 0; index < typedArray.Length(); index++)
            {
                int resId = typedArray.GetResourceId(index, 0);
                var _function = new FunctionItem(convertTexts[index],
                        resId);
                _list.Add(_function);
            }
            typedArray.Recycle();
             _gridView.OnItemClickListener = this;
            _adapter = new CustomAdapter(this,
                    Resource.Layout.function_item, _list, _gridView);
            _gridView.SetAdapter(_adapter);
        }
    }
}
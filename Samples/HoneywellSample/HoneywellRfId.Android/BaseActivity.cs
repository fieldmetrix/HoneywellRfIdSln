//using Android.Support.V7.App;
using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Java.Lang;
using Xamarin.Essentials;

namespace HoneywellRfId
{
    [Activity(Label = "BaseActivity")]
    public class BaseActivity : AppCompatActivity
    {
        private string ClsName;
        private bool _showLog = true;
        private Toast _toast;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ClsName = this.Class.SimpleName + ":";
        }

        public void ShowToast(int strId)
        {
            ShowToast(strId, false);
        }

        public void ShowToast(string s)
        {
            ShowToast(s, false);
        }

        public void ShowToast(int strId, bool longTime)
        {
            ShowToast(GetString(strId), longTime);
        }

        public void ShowToast(string s, bool longTime)
        {
            if (Thread.CurrentThread() == Looper.MainLooper.Thread)
            {
                DoShowToast(s, longTime);
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DoShowToast(s, longTime);

                });
        }
    }

    private void DoShowToast(string s, bool longTime)
    {
        if (_toast == null)
        {
                _toast = Toast.MakeText(ApplicationContext, s,
                    longTime ? ToastLength.Long : ToastLength.Short);
        }
        else
        {
                _toast.SetText(s);
        }

            _toast.Show();
    }
}
}
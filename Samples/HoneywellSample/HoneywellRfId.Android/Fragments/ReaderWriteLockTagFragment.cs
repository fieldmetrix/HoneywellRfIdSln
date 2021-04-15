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

namespace HoneywellRfId.Fragments
{
    public class ReaderWriteLockTagFragment : BaseFragment
    {
        public static ReaderWriteLockTagFragment NewInstance()
        {
            ReaderWriteLockTagFragment readerWriteLockTagFragment = new ReaderWriteLockTagFragment();
            return readerWriteLockTagFragment;
        }
    }
}
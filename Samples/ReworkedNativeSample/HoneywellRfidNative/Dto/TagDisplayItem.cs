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

namespace HoneywellRfidNative.Dto
{
    public class TagDisplayItem
    {
        public string Epc { get; set; }
        public int Rssi { get; set; }
        public int Count { get; set; }
    }
}
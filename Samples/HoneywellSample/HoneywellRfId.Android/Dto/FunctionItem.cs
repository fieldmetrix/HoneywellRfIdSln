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

namespace HoneywellRfId.Dto
{
    public class FunctionItem
    {
        public string FunctionName { get; set; }
        public int PictureSrc { get; set; }

        public FunctionItem(string functionName, int pictureSrc)
        {
            FunctionName = functionName;
            PictureSrc = pictureSrc;
        }
    }
}
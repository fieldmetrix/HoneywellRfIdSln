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
using Com.Honeywell.Rfidservice;
using HoneywellRfId.Dto;
using HoneywellRfId.Utils;
using static Android.Views.ViewGroup;

namespace HoneywellRfId.Adapters
{
    public class CustomAdapter : ArrayAdapter<FunctionItem>
    {
        private int _resourceId;
        private GridView _gridview;

        public CustomAdapter(Context context, int resource,
                                    List<FunctionItem> objects, GridView gridView) : base(context, resource, objects)
        {
            _resourceId = resource;
            _gridview = gridView;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var functionName = GetItem(position).FunctionName;
            int pictureSrc = GetItem(position).PictureSrc;
            LayoutInflater inflater = LayoutInflater.From(Context);
            View root = inflater.Inflate(_resourceId, null);
            TextView tv = (TextView)root.FindViewById(Resource.Id.function_tv);
            View v1 = (View)root.FindViewById(Resource.Id.item_rel);
            ImageView iv = (ImageView)root
                    .FindViewById(Resource.Id.function_iv);
            tv.Text = functionName;
            iv.SetImageResource(pictureSrc);
            //                LogUtils.e("potter", "gridviewheight:" + gridview.getHeight());
            LayoutParams layoutParams = v1.LayoutParameters;
            layoutParams.Height = (int)((_gridview.Height - DensityUtil.Dip2px(
                    Context, 4.0f)) / 3);
            return root;
        }
    }
}

using Android.Content;

namespace HoneywellRfId.Utils
{
    public class DensityUtil
    {
        public static float Dip2px(Context context, float dpValue)
        {
            float scale = context.Resources.DisplayMetrics.Density;
            return dpValue * scale;
        }

        public static float Px2dip(Context context, float pxValue)
        {
            float scale = context.Resources.DisplayMetrics.Density;
            return pxValue / scale;
        }
    }
}
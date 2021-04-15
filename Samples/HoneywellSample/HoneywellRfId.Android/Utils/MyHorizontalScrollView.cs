using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace com.honeywell.bccrfid.views
{
    public class MyHorizontalScrollView : HorizontalScrollView
    {
        private float xDistance, yDistance, xLast, yLast;

        public MyHorizontalScrollView(Context context) : base(context)
        {

        }

        public MyHorizontalScrollView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {


        }

        public MyHorizontalScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            switch (ev.Action)
            {
                case MotionEventActions.Down:
                    xDistance = yDistance = 0f;
                    xLast = ev.GetX();
                    yLast = ev.GetY();
                    break;

                case MotionEventActions.Move:
                    float curX = ev.GetX();
                    float curY = ev.GetY();

                    xDistance += Math.Abs(curX - xLast);
                    yDistance += Math.Abs(curY - yLast);
                    xLast = curX;
                    yLast = curY;

                    if (xDistance < yDistance)
                    {
                        return false;
                    }

                    break;
            }

            return base.OnInterceptTouchEvent(ev);
        }
    }
}
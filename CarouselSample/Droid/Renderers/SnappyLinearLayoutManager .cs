using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CarouselSample.Droid.Toolbox;
using Xamarin.Forms;
using View = Android.Views.View;

namespace CarouselSample.Droid.Renderers
{
    public class SnappyLinearLayoutManager : LinearLayoutManager
    {
        #region Private fields

        private readonly Context _context;

        #endregion


        #region Constructors

        public SnappyLinearLayoutManager(Context context)
            : base(context, Horizontal, false)
        {
            _context = context;
        }


        #endregion


        #region Public API

        public int GetSnappyScrollPosition()
        {
            if (ChildCount == 0)
            {
                return 0;
            }

            View child = GetChildAt(0);
            int childPos = GetPosition(child);

            if (Math.Abs(child.Left) > child.MeasuredWidth / 2)
            {
                // Scrolled first view more than halfway offscreen
                return childPos + 1;
            }

            return childPos;
        }

        public int GetNextSnappyScrollPosition(bool isForward)
        {
            if (ChildCount == 0)
            {
                return 0;
            }

            View child = GetChildAt(0);
            int childPos = GetPosition(child);

            return isForward ? childPos + 1 : childPos;
        }

        #endregion

        #region Parent override

        public override void SmoothScrollToPosition(RecyclerView recyclerView, RecyclerView.State state, int position)
        {
            var linearSmoothScroller = new SnappySmoothScroller(recyclerView.Context, this)
            {
                TargetPosition = position
            };
            _context.InvokeOnMainThread(() => StartSmoothScroll(linearSmoothScroller));
        }

        #endregion


    }
}
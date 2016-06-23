using System;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Util;

namespace CarouselSample.Droid.Renderers
{
    internal class SnappySmoothScroller : LinearSmoothScroller
    {
        #region Private fields

        private readonly WeakReference<SnappyLinearLayoutManager> _layoutManager;
        private const float MILLISECONDS_PER_INCH = 50f;

        #endregion

        #region Constructors

        public SnappySmoothScroller(Context context, SnappyLinearLayoutManager layoutManager)
            :base(context)
        {
            _layoutManager = new WeakReference<SnappyLinearLayoutManager>(layoutManager);
        }

        #endregion

        #region Parent override

        protected override float CalculateSpeedPerPixel(DisplayMetrics displayMetrics)
        {
            return MILLISECONDS_PER_INCH / (int)displayMetrics.DensityDpi;
        }

        public override PointF ComputeScrollVectorForPosition(int targetPosition)
        {
            SnappyLinearLayoutManager layoutManager;
            _layoutManager.TryGetTarget(out layoutManager);

            return layoutManager.ComputeScrollVectorForPosition(targetPosition);
        }
        protected override int HorizontalSnapPreference => SnapToStart;

        #endregion

    }
}
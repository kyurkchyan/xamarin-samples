using System;
using System.Collections;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using CarouselSample.Droid.Toolbox;

namespace CarouselSample.Droid.Renderers
{
    public sealed class SnappyRecyclerView : RecyclerView
    {
        #region Private fields

        private readonly int _flingThreshold;

        #endregion

        #region Constructors

        public SnappyRecyclerView(Context context, Action<int> selectItemAction)
            : base(context)
        {
            var config = ViewConfiguration.Get(Context);
            _flingThreshold = (int)(config.ScaledMinimumFlingVelocity + 0.3 * config.ScaledMaximumFlingVelocity);
            SelectItemAction = selectItemAction;
        }

        #endregion

        #region Properties

        public Action<int> SelectItemAction { get; set; }

        #endregion

        #region Parent override

        public override bool Fling(int velocityX, int velocityY)
        {
            var layout = GetLayoutManager() as SnappyLinearLayoutManager;
            if (layout != null)
            {
                if (Math.Abs(velocityX) > _flingThreshold)
                    UpdateCurrentItem(true, velocityX >= 0);
                else
                    UpdateCurrentItem();
            }
            return base.Fling(velocityX, velocityY);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            var layout = GetLayoutManager() as SnappyLinearLayoutManager;
            if (layout != null && (e.Action == MotionEventActions.Up ||
                e.Action == MotionEventActions.Cancel))
            {
                UpdateCurrentItem();
            }
            return base.OnTouchEvent(e);
        }

        #endregion

        #region Utility methods

        private void UpdateCurrentItem(bool isNext = false, bool isForward = true)
        {
            var layout = GetLayoutManager() as SnappyLinearLayoutManager;
            if (layout == null)
                return;
            var index = isNext ? layout.GetNextSnappyScrollPosition(isForward) : layout.GetSnappyScrollPosition();
            SelectItemAction?.Invoke(index);
        }

        #endregion
    }
}
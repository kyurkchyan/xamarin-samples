using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;

namespace ImageSample.Droid.Views
{
    public class GridAutofitLayoutManager : GridLayoutManager
    {
        #region Private fields and properties

        private int mColumnWidth;
        private bool mColumnWidthChanged = true;

        #endregion

        #region Constructors

        public GridAutofitLayoutManager(IntPtr javeReference, JniHandleOwnership transfer)
            : base(javeReference, transfer)
        {
        }

        public GridAutofitLayoutManager(Context context, int columnWidth)
            /* Initially set spanCount to 1, will be changed automatically later. */
            : base(context, 1)
        {
            SetColumnWidth(checkedColumnWidth(context, columnWidth));
        }

        public GridAutofitLayoutManager(Context context, int columnWidth, int orientation, bool reverseLayout)
            /* Initially set spanCount to 1, will be changed automatically later. */
            : base(context, 1, orientation, reverseLayout)
        {
            SetColumnWidth(checkedColumnWidth(context, columnWidth));
        }

        #endregion

        public void SetColumnWidth(int newColumnWidth)
        {
            if (newColumnWidth > 0 && newColumnWidth != mColumnWidth)
            {
                mColumnWidth = newColumnWidth;
                mColumnWidthChanged = true;
            }
        }

        #region Parent override

        public override void OnLayoutChildren(RecyclerView.Recycler recycler, RecyclerView.State state)
        {
            if (mColumnWidthChanged && mColumnWidth > 0)
            {
                int totalSpace;
                if (Orientation == OrientationHelper.Vertical)
                {
                    totalSpace = Width - PaddingRight - PaddingLeft;
                }
                else
                {
                    totalSpace = Height - PaddingTop - PaddingBottom;
                }
                var spanCount = Math.Max(1, totalSpace/mColumnWidth);
                SpanCount = spanCount;
                mColumnWidthChanged = false;
            }
            base.OnLayoutChildren(recycler, state);
        }

        #endregion

        #region Utility methods

        private int checkedColumnWidth(Context context, int columnWidth)
        {
            columnWidth = columnWidth <= 0 ? 48 : columnWidth;
            columnWidth =
                (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, columnWidth, context.Resources.DisplayMetrics);
            return columnWidth;
        }

        #endregion
    }
}
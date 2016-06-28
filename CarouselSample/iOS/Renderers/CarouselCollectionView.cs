using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using CoreGraphics;
using UIKit;

namespace CarouselSample.iOS.Renderers
{
    public sealed class CarouselCollectionView : UICollectionView
    {
        #region Private fields

        private readonly UICollectionViewFlowLayout _layout;
        private nfloat _previousWidth;
        private nfloat _previousHeight;

        #endregion

        #region Constructors

        public CarouselCollectionView(CGRect frame)
            : base(frame, new UICollectionViewFlowLayout()
            {
                ScrollDirection = UICollectionViewScrollDirection.Horizontal
            })
        {
            RegisterClassForCell(typeof(CarouselCell), nameof(CarouselCell));
            _layout = (UICollectionViewFlowLayout)CollectionViewLayout;
            ScrollEnabled = true;
            DecelerationRate = DecelerationRateFast;
            ShowsHorizontalScrollIndicator = false;
        }

        #endregion

        #region Parent override

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (_previousWidth != Bounds.Width ||
                    _previousHeight != Bounds.Height)
            {
                var source = Source as CarouselSource;
                source?.SnapToCurrentIndex(false);
                _previousWidth = Bounds.Width;
                _previousHeight = Bounds.Height;
            }
        }

        #endregion

        #region Properties

        private CGSize _cellSize;
        public CGSize CellSize
        {
            get
            {
                return _cellSize;
            }
            set
            {
                if (_cellSize != value)
                {
                    _cellSize = value;
                    _layout.ItemSize = _cellSize;
                }
            }
        }

        #endregion
    }
}
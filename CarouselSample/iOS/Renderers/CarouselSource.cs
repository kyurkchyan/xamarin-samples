using System;
using System.Diagnostics;
using System.Linq;
using Carousels;
using CarouselSample.Controls;
using CarouselSample.iOS.Toolbox;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace CarouselSample.iOS.Renderers
{
    internal class CarouselSource : UICollectionViewSource
    {
        #region Private fields

        private readonly Carousel _carousel;
        private readonly WeakReference<CarouselCollectionView> _collectionView;
        private nfloat _velocity;
        private CGPoint _offset;
        private readonly nfloat _velocityThreshold = 0;
        private readonly nfloat _snapAnimationDuration = 0.2f;
        
        #endregion


        #region Constructors

        public CarouselSource(Carousel carousel, CarouselCollectionView collectionView)
        {
            _collectionView = new WeakReference<CarouselCollectionView>((CarouselCollectionView)collectionView);
            _carousel = carousel;
        }

        #endregion

        #region Public API

        public int CurrentIndex { get; private set; }

        public void SnapToCurrentIndex(bool animated = true)
        {
            if (!(CurrentIndex >= 0 && _carousel?.Items?.Count > 0))
                return;

            CarouselCollectionView collectionView = null;
            if (!_collectionView.TryGetTarget(out collectionView))
                return;
            var index = NSIndexPath.FromRowSection(CurrentIndex, 0);
            var rect = collectionView.GetLayoutAttributesForItem(index);
            var duration = animated ? 0.2f : 0f;
            UIView.Animate(duration, () => {
                collectionView.ContentOffset = rect.Frame.Location;
            });
        }

        #endregion


        #region UICollectionViewSource

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return _carousel?.Items?.Count ?? 0;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(nameof(CarouselCell), indexPath) as CarouselCell;
            cell.Prepare(_carousel.ItemTemplate);
            var item = _carousel.Items[indexPath.Row];
            cell.Update(item);
            return cell;
        }

        public override void WillEndDragging(UIScrollView scrollView, CGPoint velocity, ref CGPoint targetContentOffset)
        {
            _velocity = velocity.X;
            _offset = scrollView.ContentOffset;
            var position = GetSnapScrollPosition(scrollView, _velocity, _offset);
            if (position != null)
            {
                targetContentOffset = position.Value;
            }
        }

        #endregion

        #region Utility methods

        private CGPoint? GetSnapScrollPosition(UIScrollView scrollView, nfloat velocity, CGPoint offset)
        {
            CarouselCollectionView collectionView = null;
            if (!_collectionView.TryGetTarget(out collectionView))
                return null;

            var count = _carousel?.Items?.Count;
            if (!(count >= 0))
                return null;


            var location = velocity >= 0 ? offset : new CGPoint(offset.X + collectionView.Frame.Width, 0);
            var currentIndex = collectionView.IndexPathForItemAtPoint(location);
            if (currentIndex == null)
                return null;

            var currentRect = collectionView.GetLayoutAttributesForItem(currentIndex);
            NSIndexPath newIndex = currentIndex;

            if ((Math.Abs(velocity) > _velocityThreshold && velocity > 0 ||
                 offset.X > currentRect.Frame.Left + currentRect.Bounds.Size.Width / 2) &&
                currentIndex.Row < count - 1)
            {
                newIndex = NSIndexPath.FromRowSection(currentIndex.Row + 1, 0);
            }
            else if ((Math.Abs(velocity) > _velocityThreshold && velocity < 0 ||
                      offset.X < currentRect.Frame.Left - currentRect.Bounds.Size.Width / 2) &&
                     currentIndex.Row > 0)
            {
                newIndex = NSIndexPath.FromRowSection(currentIndex.Row - 1, 0);
            }

            UICollectionViewLayoutAttributes newRect = currentRect;
            if (newIndex != currentIndex)
            {
                CurrentIndex = newIndex.Row;
                newRect = collectionView.GetLayoutAttributesForItem(newIndex);
                var item = _carousel.Items[CurrentIndex];
                _carousel.Current = item;
            }

            return newRect.Frame.Location;
        }

        #endregion

    }
}
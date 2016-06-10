using System;
using System.Linq;
using Carousels;
using CarouselSample.Controls;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace CarouselSample.iOS.Renderers
{
    internal class CarouselDataSource : iCarouselDataSource
    {
        #region Private fields

        private readonly Carousel _carousel;

        #endregion


        #region Constructors

        public CarouselDataSource(Carousel carousel)
        {
            _carousel = carousel;
        }

        #endregion


        #region iCarouselDataSource

        // let the carousel know how many items to render
        public override nint GetNumberOfItems(iCarousel carousel)
        {
            // return the number of items in the data
            return _carousel?.Items?.Count ?? 0;
        }

        // create the view each item in the carousel
        public override UIView GetViewForItem(iCarousel carousel, nint index, UIView view)
        {
            UIView container = null;
            VisualElementRenderer<VisualElement> nativeView = null;
            if (view == null)
            {
                var item = (View)_carousel.ItemTemplate.CreateContent();
                var renderer = Platform.CreateRenderer(item);
                nativeView = (VisualElementRenderer<VisualElement>)renderer.NativeView;
                nativeView.AutoresizingMask = UIViewAutoresizing.All;
                nativeView.Tag = 1000;
                var width = nativeView.Element.WidthRequest > 0 ? nativeView.Element.WidthRequest : carousel.Bounds.Width;
                var height = nativeView.Element.HeightRequest > 0 ? nativeView.Element.HeightRequest : carousel.Bounds.Height;
                width = Math.Min(width, _carousel.MaxWidth);
                container = new UIView(new CGRect(0, 0, width, height));
                container.BackgroundColor = UIColor.Clear;
                container.AddSubview(nativeView);
                nativeView.Element.Layout(new Rectangle(0, 0, width, height));
            }
            else
            {
                container = view;
                nativeView = (VisualElementRenderer<VisualElement>)(view.ViewWithTag(1000));
            }

            nativeView.Element.BindingContext = _carousel.Items[(int)index];

            return container;
        }

        #endregion

    }
}
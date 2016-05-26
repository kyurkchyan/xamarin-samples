using System;
using System.ComponentModel;
using CarouselSample.Controls;
using CarouselSample.iOS.Renderers;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof (CarouselLayout), typeof (CarouselLayoutRenderer))]

namespace CarouselSample.iOS.Renderers
{
    public class CarouselLayoutRenderer : ScrollViewRenderer
    {
        #region Private fields and properties

        UIScrollView _native;

        #endregion

        #region Constructors

        public CarouselLayoutRenderer()
        {
            PagingEnabled = true;
            ShowsHorizontalScrollIndicator = false;
        }

        #endregion

        #region Parent override

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
                return;

            _native = (UIScrollView) NativeView;
            _native.Scrolled += NativeScrolled;
            e.NewElement.PropertyChanged += ElementPropertyChanged;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            ScrollToSelection(false);
        }

        #endregion

        #region Utility methods

        void NativeScrolled(object sender, EventArgs e)
        {
            var center = _native.ContentOffset.X + _native.Bounds.Width/2;
            ((CarouselLayout) Element).SelectedIndex = (int) center/(int) _native.Bounds.Width;
        }

        void ElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CarouselLayout.SelectedIndexProperty.PropertyName && !Dragging)
            {
                ScrollToSelection(false);
            }
        }

        void ScrollToSelection(bool animate)
        {
            if (Element == null)
                return;

            _native.SetContentOffset(new CGPoint(_native.Bounds.Width*
                                                 Math.Max(0, ((CarouselLayout) Element).SelectedIndex),
                _native.ContentOffset.Y),
                animate);
        }

        #endregion
    }
}
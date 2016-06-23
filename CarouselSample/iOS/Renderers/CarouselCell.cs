using System;
using System.Collections.Generic;
using System.Text;
using CarouselSample.iOS.Toolbox;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace CarouselSample.iOS.Renderers
{
    public sealed class CarouselCell : UICollectionViewCell
    {
        #region Private fields

        private UIView _nativeView;
        private View _view;
        private CGSize _lastSize;

        #endregion

        #region Constructors

        [Export("initWithFrame:")]
        public CarouselCell(CGRect frame)
            : base(frame)
        {

        }

        public CarouselCell(IntPtr handle)
            : base(handle)
        {
            
        }

        #endregion

        #region Public API

        public void Prepare(DataTemplate template)
        {
            if (_view == null)
            {
                _view = (View)template.CreateContent();
                var renderer = _view.GetOrCreateRenderer();
                _nativeView = renderer.NativeView;
                _nativeView.AutoresizingMask = UIViewAutoresizing.All;
                _nativeView.ContentMode = UIViewContentMode.ScaleToFill;
                ContentView.AddSubview(_nativeView);
            }
        }

        public void Update(object bindingContext)
        {
            _view.BindingContext = bindingContext;
        }

        #endregion

        #region Parent override

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (_lastSize.Equals(CGSize.Empty) || !_lastSize.Equals(Frame.Size))
            {

                _view.Layout(Frame.ToRectangle());
                _lastSize = Frame.Size;
            }

            _nativeView.Frame = ContentView.Bounds;

        }

        //public override void ApplyLayoutAttributes(UICollectionViewLayoutAttributes layoutAttributes)
        //{
        //    var attributes = (CarouselLayoutAttributes) layoutAttributes;
        //    this.Layer.AddAnimation(attributes.TransformAnimation, "transform");

        //}

        #endregion
    }
}

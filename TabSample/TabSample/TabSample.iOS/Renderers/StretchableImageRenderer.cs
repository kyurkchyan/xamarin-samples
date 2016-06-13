using System;
using System.ComponentModel;
using TabSample.Controls;
using TabSample.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(StretchableImage), typeof(StretchableImageRenderer))]
namespace TabSample.iOS.Renderers
{
    public class StretchableImageRenderer : ViewRenderer<StretchableImage, UIImageView>
    {
        #region Parent override

        protected override async void OnElementChanged(ElementChangedEventArgs<StretchableImage> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
                return;
            if (base.Control == null)
            {
                var imageView = new UIImageView();
                base.SetNativeControl(imageView);
            }
            UpdateImage();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == StretchableImage.PathProperty.PropertyName ||
                e.PropertyName == StretchableImage.CapsProperty.PropertyName)
            {
                UpdateImage();
            }
        }

        #endregion

        #region Utility methods

        private void UpdateImage()
        {
            UIImage stretchableImage = null;
            if (Element != null)
            {
                var image = Element.Path != null ? UIImage.FromFile(Element.Path) : null;
                stretchableImage = image?.StretchableImage((nint)Element.Caps.Width, (nint)Element.Caps.Height);
            }
            if (Control != null)
            {
                Control.Image = stretchableImage;
            }
        }

        #endregion
    }
}

using Android.Graphics;
using Android.Graphics.Drawables;
using TabSample.Controls;
using TabSample.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly:ExportRenderer(typeof(StretchableImage), typeof(StretchableImageRenderer))]
namespace TabSample.Droid.Renderers
{
    public class StretchableImageRenderer : ViewRenderer<StretchableImage, Android.Widget.ImageView>
    {
        #region Parent override

        protected override void OnElementChanged(ElementChangedEventArgs<StretchableImage> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
                return;
            if (base.Control == null)
            {
                Android.Widget.ImageView imageView = new Android.Widget.ImageView(base.Context);
                base.SetNativeControl(imageView);
            }

            UpdateImage();
        }

        #endregion

        #region Utility methods

        private void UpdateImage()
        {
            NinePatchDrawable image = null;
            if ( Element?.Path != null)
            {

                var resourceId = GetResourceId();
                if (resourceId > 0)
                {
                    var sourceImage = BitmapFactory.DecodeResource(Resources, resourceId);
                    var chunk = sourceImage.GetNinePatchChunk();
                    image = new NinePatchDrawable(Resources, sourceImage, chunk, new Rect(), null);
                }
            }
            Control?.SetBackground(image);
        }

        private int GetResourceId()
        {
            if (Element?.Path == null)
                return 0;
            string name = Element?.Path;
            var index = name.LastIndexOf(".");
            if (index >= 0)
                name = name.Remove(index).ToLower();
            try
            {
                var res = typeof(Resource.Drawable);
                var field = res.GetField(name);
                int drawableId = (int)field.GetValue(null);
                return drawableId;
            }
            catch
            {
                return 0;
            }
        }

        #endregion
    }
}
using System.ComponentModel;
using FontSample.Droid.Renderers;
using FontSample.Droid.Toolbox;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ButtonRenderer = FontSample.Droid.Renderers.ButtonRenderer;

[assembly: ExportRenderer(typeof(Button), typeof(ButtonRenderer))]

namespace FontSample.Droid.Renderers
{
    class ButtonRenderer : Xamarin.Forms.Platform.Android.ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
                return;
            Control.SetAllCaps(false);
            UpdateFont(Element);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (Element == null || Control == null)
                return;
            if (e.PropertyName == Button.FontProperty.PropertyName || 
                e.PropertyName == Button.FontFamilyProperty.PropertyName)
            {
                UpdateFont(Element);
            }
        }

        private void UpdateFont(Button button)
        {
            Control.Typeface = Context.TryGetFont(button.FontFamily, button.FontAttributes);
        }
    }
}
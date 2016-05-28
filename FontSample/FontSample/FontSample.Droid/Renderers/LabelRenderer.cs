using System;
using System.ComponentModel;
using Android.Graphics;
using FontSample.Droid.Renderers;
using FontSample.Droid.Toolbox;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Label), typeof(LabelRender))]
namespace FontSample.Droid.Renderers
{
    public class LabelRender : LabelRenderer
    {
        #region Parent override
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || this.Element == null)
                return;

            var label = (Label)Element;

            UpdateFont(label);

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (Element == null || Control == null)
                return;
            var label = (Label)Element;
            if (e.PropertyName == Label.FontFamilyProperty.PropertyName)
            {
                UpdateFont(label);
            }
        }

        #endregion

        #region Utility methods

        private void UpdateFont(Label Label)
        {
            if (!string.IsNullOrEmpty(Label.FontFamily))
            {
                Control.Typeface = Context.TryGetFont(Label.FontFamily, Label.FontAttributes);
            }
        }


        #endregion
    }
}

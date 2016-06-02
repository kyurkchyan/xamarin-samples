using System;
using Android.Graphics.Drawables;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace EntryExSample.Droid.Renderers
{
    public class BorderRenderer : IDisposable
    {
        #region Parent override

        private GradientDrawable _background;

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_background != null)
                {
                    _background.Dispose();
                    _background = null;
                }
            }
        }

        #endregion

        #region Public API

        public Drawable GetBorderBackground(Color borderColor, Color backgroundColor, float borderWidth, float borderRadius)
        {
            if (_background != null)
            {
                _background.Dispose();
                _background = null;
            }
            borderWidth = borderWidth > 0 ? borderWidth : 0;
            borderRadius = borderRadius > 0 ? borderRadius : 0;
            borderColor = borderColor != Color.Default ? borderColor : Color.Transparent;
            backgroundColor = backgroundColor != Color.Default ? backgroundColor : Color.Transparent;

            var strokeWidth = Xamarin.Forms.Forms.Context.ToPixels(borderWidth);
            var radius = Xamarin.Forms.Forms.Context.ToPixels(borderRadius);
            _background = new GradientDrawable();
            _background.SetColor(backgroundColor.ToAndroid());
            if (radius > 0)
                _background.SetCornerRadius(radius);
            if (borderColor != Color.Transparent && strokeWidth > 0)
            {
                _background.SetStroke((int)strokeWidth, borderColor.ToAndroid());
            }
            return _background;
        }

        #endregion
    }
}
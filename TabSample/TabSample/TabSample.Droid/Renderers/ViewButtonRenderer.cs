using System;
using System.ComponentModel;
using Android.Views;
using TabSample.Controls;
using TabSample.Droid.Renderers;
using TabSample.Droid.Toolbox;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ViewButton), typeof(ViewButtonRenderer))]
namespace TabSample.Droid.Renderers
{
    public class ViewButtonRenderer : VisualElementRenderer<ContentView>
    {
        #region Private fields

        private bool _isEnabled;

        #endregion

        #region Parent override

        protected override void OnElementChanged(ElementChangedEventArgs<ContentView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
                return;

            ToggleIsEnabled();
            _isEnabled = Element.IsEnabled;
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            this.DisableChildren();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (Element == null)
                return;
            if (e.PropertyName == Xamarin.Forms.Frame.IsEnabledProperty.PropertyName)
            {
                if (_isEnabled != Element.IsEnabled)
                    ToggleIsEnabled();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (Enabled)
                    RemoveTouchListener();
            }
        }

        #endregion


        #region Utility Methods

        private void OnTouch(object sender, TouchEventArgs args)
        {

            if (args.Event.Action == MotionEventActions.Down)
            {
                Alpha = 0.5f;
            }
            else if (args.Event.Action == MotionEventActions.Up || args.Event.Action == MotionEventActions.Cancel)
            {
                Alpha = 1;

                var x = args.Event.GetX();
                var y = args.Event.GetY();
                if (ContainsPoint((float)x, (float)y))
                {
                    var button = Element as ViewButton;
                    if (button?.Command?.CanExecute(null) == true)
                        button?.Command?.Execute(null);
                    button?.InvokeClicked(button, EventArgs.Empty);
                }
            }
        }

        private void ToggleIsEnabled()
        {
            _isEnabled = Element.IsEnabled;
            if (Element.IsEnabled)
            {
                Alpha = 1;
                AddTouchListener();
            }
            else
            {
                Alpha = 0.5f;
                RemoveTouchListener();
            }
        }

        private void AddTouchListener()
        {
            RootView.Touch += OnTouch;
        }

        private void RemoveTouchListener()
        {
            RootView.Touch -= OnTouch;
        }

        private bool ContainsPoint(float x, float y)
        {
            var metrics = Context.Resources.DisplayMetrics;

            float px = Context.ConvertPixelsToDp(x);
            float py = Context.ConvertPixelsToDp(y);
            var frame = new Rectangle(0, 0, Element.Bounds.Width, Element.Bounds.Height);
            var contains = frame.Contains(px, py);
            return contains;
        }

        #endregion
    }
}
using System;
using System.ComponentModel;
using CoreGraphics;
using UIKit;
using ViewButtonSample.Controls;
using ViewButtonSample.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(ViewButton), typeof(ViewButtonRenderer))]
namespace ViewButtonSample.iOS.Renderers
{
    public class ViewButtonRenderer : VisualElementRenderer<ContentView>
    {
        #region Private fields

        private UILongPressGestureRecognizer _gesture;
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

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == Xamarin.Forms.VisualElement.IsEnabledProperty.PropertyName)
            {
                if(_isEnabled != Element.IsEnabled)
                    ToggleIsEnabled();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (_gesture != null)
                {
                    _gesture.Dispose();
                    _gesture = null;
                }
            }
        }

        #endregion

        
        #region Utility Methods

        private void SelectGestureRecognized(UILongPressGestureRecognizer recognizer)
        {
            var view = recognizer.View;

            if (recognizer.State == UIGestureRecognizerState.Began)
            {
                Alpha = 0.5f;
            }
            else if (recognizer.State == UIGestureRecognizerState.Ended)
            {
                Alpha = 1;

                CGPoint touchedPoint = recognizer.LocationInView(view);

                if (Bounds.Contains(touchedPoint))
                {
                    var button = Element as ViewButton;
                    if(button?.Command?.CanExecute(null) == true)
                        button?.Command?.Execute(null);
                    button?.InvokeClicked(this, EventArgs.Empty);
                }
            }
        }

        private void ToggleIsEnabled()
        {
            _isEnabled = Element.IsEnabled;
            if (Element.IsEnabled)
            {
                Alpha = 1;
                AddGesture();
            }
            else
            {
                Alpha = 0.5f;
                RemoveGesture();
            }
        }

        private void AddGesture()
        {
            if (_gesture == null)
            {
                _gesture = new UILongPressGestureRecognizer(SelectGestureRecognized);
                _gesture.MinimumPressDuration = 0;
            }
            AddGestureRecognizer(_gesture);
        }

        private void RemoveGesture()
        {
            if (_gesture != null)
            {
                RemoveGestureRecognizer(_gesture);
            }
        }

        #endregion
    }
}

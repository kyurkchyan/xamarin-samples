using System;
using System.ComponentModel;
using Android.Animation;
using TabSample.Controls;
using TabSample.Droid.Renderers;
using TabSample.Droid.Toolbox;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof (ScrollViewEx), typeof (ScrollViewExRenderer))]

namespace TabSample.Droid.Renderers
{
    public class ScrollViewExRenderer : ScrollViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                e.OldElement.PropertyChanged -= PropertyChanged;
                ChildViewAdded -= OnChildViewAdded;
                return;
            }

            if (Element == null)
                return;

            Element.PropertyChanged += PropertyChanged;
            ChildViewAdded += OnChildViewAdded;

            var scrollView = (ScrollViewEx) Element;
            UpdateHorizontalScroll(scrollView);
            UpdateVerticalScroll(scrollView);
        }

        private void OnChildViewAdded(object sender, ChildViewAddedEventArgs childViewAddedEventArgs)
        {
            var scrollView = (ScrollViewEx)Element;
            UpdateHorizontalScroll(scrollView);
            UpdateVerticalScroll(scrollView);
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var scrollView = (ScrollViewEx) Element;
            if (e.PropertyName == ScrollViewEx.ShowHorizontalScrollIndicatorProperty.PropertyName)
            {
                UpdateHorizontalScroll(scrollView);
            }

            if (e.PropertyName == ScrollViewEx.ShowVerticalScrollIndicatorProperty.PropertyName)
            {
                UpdateVerticalScroll(scrollView);
            }
        }

        private void UpdateHorizontalScroll(ScrollViewEx scrollView)
        {
            if (ChildCount > 0)
            {
                GetChildAt(0).HorizontalScrollBarEnabled = scrollView.ShowHorizontalScrollIndicator;
            }
        }

        private void UpdateVerticalScroll(ScrollViewEx scrollView)
        {
            if (ChildCount > 0)
            {
                GetChildAt(0).VerticalScrollBarEnabled = scrollView.ShowVerticalScrollIndicator;
            }
        }
    }
}
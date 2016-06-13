using System.ComponentModel;
using TabSample.Controls;
using TabSample.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof (ScrollViewEx), typeof (ScrollViewExRenderer))]

namespace TabSample.iOS.Renderers
{
    public class ScrollViewExRenderer : ScrollViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                e.OldElement.PropertyChanged -= PropertyChanged;
                return;
            }

            if (Element == null)
                return;

            Element.PropertyChanged += PropertyChanged;

            var scrollView = (ScrollViewEx) Element;

            UpdateHorizontalScroll(scrollView);
            UpdateVerticalScroll(scrollView);
            UpdateBounce(scrollView);
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
            ShowsHorizontalScrollIndicator = scrollView.ShowHorizontalScrollIndicator;
        }

        private void UpdateVerticalScroll(ScrollViewEx scrollView)
        {
            ShowsVerticalScrollIndicator = scrollView.ShowVerticalScrollIndicator;
        }

        private void UpdateBounce(ScrollViewEx scrollViewEx)
        {
            Bounces = scrollViewEx.BounceEnabled;
        }
    }
}
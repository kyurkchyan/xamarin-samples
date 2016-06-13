using System;
using TabSample.Controls;
using TabSample.iOS.Renderers;
using TabSample.Toolbox;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PageViewContainer), typeof(PageViewContainerRenderer))]
namespace TabSample.iOS.Renderers
{
    public class PageViewContainerRenderer : ViewRenderer<PageViewContainer, ViewControllerContainer>
    {
        public PageViewContainerRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<PageViewContainer> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.ViewController = null;
            }

            if (e.NewElement != null)
            {
                var viewControllerContainer = new ViewControllerContainer(Bounds);
                SetNativeControl(viewControllerContainer);
            }


        }

        void ChangePage(Page page)
        {
            if (page != null)
            {

                page.Parent = Element.GetParentPage();
                
                var pageRenderer = Platform.GetRenderer(page) ?? Platform.CreateRenderer(page);
                var viewController = pageRenderer.ViewController;

                var parentPage = Element.GetParentPage();
                var renderer = Platform.GetRenderer(parentPage);

                Control.ParentViewController = renderer.ViewController;
                Control.ViewController = viewController;
            }
            else
            {
                if (Control != null)
                {
                    Control.ViewController = null;
                }
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var page = Element != null ? Element.Content : null;
            if (page != null)
            {
                page.Layout(new Rectangle(0, 0, Bounds.Width, Bounds.Height));
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "Content" || e.PropertyName == "Renderer")
            {
                Device.BeginInvokeOnMainThread(() => ChangePage(Element != null ? Element.Content : null));
            }
        }

    }
}

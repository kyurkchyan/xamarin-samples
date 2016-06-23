using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace CarouselSample.iOS.Toolbox
{
    public static class ViewExtensions
    {
        public static IVisualElementRenderer GetOrCreateRenderer(this VisualElement element)
        {
            IVisualElementRenderer renderer = Platform.GetRenderer(element);
            if (renderer == null)
            {
                renderer = Platform.CreateRenderer(element);
            }
            return renderer;
        }
    }
}

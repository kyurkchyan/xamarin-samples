using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace CarouselSample.Droid.Toolbox
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

        public static void InvokeOnMainThread(this Context c, Action a)
        {
            using (var h = new Handler(Looper.MainLooper))
            {
                h.Post(a);
            }
        }
    }
}
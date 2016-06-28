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
using CarouselSample.Droid.Toolbox;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Xamarin.Forms.View;

namespace CarouselSample.Droid.Renderers
{
    public class CarouselViewHolder : ViewGroup
    {
        #region Private fields

        private ViewGroup _nativeView;
        private View _view;

        #endregion

        #region Constructors

        public CarouselViewHolder(Context context, DataTemplate template) : base(context)
        {
            context.InvokeOnMainThread(() =>
            {
                _view = (View)template.CreateContent();
                var renderer = _view.GetOrCreateRenderer();
                _nativeView = renderer.ViewGroup;
                AddView(_nativeView);
            });

        }

        #endregion

        #region Public API

        public void Update(object bindingContext)
        {
            Context.InvokeOnMainThread(() =>
            {
                _view.BindingContext = bindingContext;
            });
        }

        #endregion

        #region Parent override

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            using (var h = new Handler(Looper.MainLooper))
            {
                h.Post(() =>
                {
                    double width = base.Context.FromPixels((double)(r - l));
                    double height = base.Context.FromPixels((double)(b - t));
                    var size = new Size(width, height);
                    var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
                    var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);
                    _nativeView.Measure(msw, msh);
                    _nativeView.Layout(0, 0, r - l, b - t);

                    var layout = _view as Layout<View>;
                    if (layout != null)
                    {
                        layout.Layout(new Rectangle(0, 0, width, height));
                        layout.ForceLayout();
                        FixChildLayouts(layout);
                    }
                });
            }
        }

        #endregion

        #region Utility methods

        private void FixChildLayouts(Layout<View> layout)
        {
            foreach (var child in layout.Children)
            {
                if (child is Layout<View>)
                {
                    ((Layout<View>)child).ForceLayout();
                    FixChildLayouts(child as Layout<View>);
                }
            }
        }

        #endregion

    }
}
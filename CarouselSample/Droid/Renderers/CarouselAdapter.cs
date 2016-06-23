using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CarouselSample.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace CarouselSample.Droid.Renderers
{
    public class CarouselAdapter : RecyclerView.Adapter
    {
        #region Private fields

        private readonly Carousel _carousel;

        #endregion

        #region Constructors

        public CarouselAdapter(Carousel carousel)
        {
            _carousel = carousel;
            var frame = _carousel.Bounds;
            LayoutParams = new ViewGroup.LayoutParams((int)Forms.Context.ToPixels(frame.Width),
                (int)Forms.Context.ToPixels(frame.Height));
        }

        #endregion

        #region Properties

        public ViewGroup.LayoutParams LayoutParams { get; private set; }

        #endregion

        #region Adapter

        public override int ItemCount => _carousel?.Items?.Count ?? 0;

        public class RecyclerViewCarouselViewHolder : RecyclerView.ViewHolder
        {
            public CarouselViewHolder ViewHolder { get; set; }

            public RecyclerViewCarouselViewHolder(CarouselViewHolder view) : base(view)
            {
                ViewHolder = view;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = new CarouselViewHolder(Forms.Context, _carousel.ItemTemplate);
            view.LayoutParameters = LayoutParams;

            var viewHolder = new RecyclerViewCarouselViewHolder(view);
            return viewHolder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = (RecyclerViewCarouselViewHolder) holder;
            viewHolder.ViewHolder.Update(_carousel.Items[position]);
        }

        #endregion

    }
}
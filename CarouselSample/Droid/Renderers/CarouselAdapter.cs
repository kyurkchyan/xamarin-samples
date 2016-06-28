using Android.Support.V7.Widget;
using Android.Views;
using CarouselSample.Controls;
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
            LayoutParams = new ViewGroup.LayoutParams((int)Xamarin.Forms.Forms.Context.ToPixels(frame.Width),
                (int)Xamarin.Forms.Forms.Context.ToPixels(frame.Height));
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
            var view = new CarouselViewHolder(Xamarin.Forms.Forms.Context, _carousel.ItemTemplate);
            view.LayoutParameters = LayoutParams;

            var viewHolder = new RecyclerViewCarouselViewHolder(view);
            return viewHolder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = (RecyclerViewCarouselViewHolder)holder;
            viewHolder.ViewHolder.Update(_carousel.Items[position]);
        }

        #endregion

    }
}
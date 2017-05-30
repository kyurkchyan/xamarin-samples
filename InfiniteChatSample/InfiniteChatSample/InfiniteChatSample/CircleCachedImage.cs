using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using Xamarin.Forms;

namespace InfiniteChatSample
{
    public class CircleCachedImage : CachedImage
    {
        public CircleCachedImage()
        {
            Transformations.Add(new CircleTransformation());
            WidthRequest = 50;
            HeightRequest = 50;
        }
    }
}
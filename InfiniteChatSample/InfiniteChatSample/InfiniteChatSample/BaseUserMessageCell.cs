using FFImageLoading.Forms;
using InfiniteChatSample.ViewModels;
using Xamarin.Forms;

namespace InfiniteChatSample
{
    public class BaseUserMessageCell : ViewCell
    {
        protected virtual CachedImage UserImage { get; }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var message = BindingContext as MessageViewModel;
            var imageUrl = message?.ProfilePicture;
            UserImage.Source = imageUrl;
            ForceUpdateSize();
        }
    }
}
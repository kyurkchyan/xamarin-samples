using System.Linq;
using InfiniteChatSample.Droid;
using InfiniteChatSample;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly:ExportRenderer(typeof(ChatStream), typeof(ChatStreamRenderer))]

namespace InfiniteChatSample.Droid
{
    public class ChatStreamRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            if(e.NewElement != null)
            {
                var chatStream = (ChatStream) e.NewElement;
                chatStream.GetFirstVisibleItemIndex = () => Control?.FirstVisiblePosition;
                chatStream.GetLastVisibleItemIndex = () => Control?.LastVisiblePosition;
            }
        }
    }
}
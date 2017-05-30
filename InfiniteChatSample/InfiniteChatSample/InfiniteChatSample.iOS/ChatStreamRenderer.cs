using System.Linq;
using InfiniteChatSample;
using InfiniteChatSample.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(ChatStream), typeof(ChatStreamRenderer))]

namespace InfiniteChatSample.iOS
{
    public class ChatStreamRenderer : ListViewRenderer
    {
        public ChatStreamRenderer()
        {
            AnimationsEnabled = false;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            if(e.NewElement != null)
            {
                var chatStream = (ChatStream) e.NewElement;
                chatStream.GetFirstVisibleItemIndex = () => Control?.IndexPathsForVisibleRows?.FirstOrDefault()?.Row;
                chatStream.GetLastVisibleItemIndex = () => Control?.IndexPathsForVisibleRows?.LastOrDefault()?.Row;
            }
        }
    }
}
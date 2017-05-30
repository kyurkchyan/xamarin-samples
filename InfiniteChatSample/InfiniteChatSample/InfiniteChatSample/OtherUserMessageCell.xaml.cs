using FFImageLoading.Forms;
using Xamarin.Forms;

namespace InfiniteChatSample
{
    public partial class OtherUserMessageCell
    {
        public OtherUserMessageCell()
        {
            InitializeComponent();
        }

        protected override CachedImage UserImage => ProfilePicture;
    }
}
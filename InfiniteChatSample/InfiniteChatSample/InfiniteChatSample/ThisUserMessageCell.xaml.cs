using FFImageLoading.Forms;

namespace InfiniteChatSample
{
    public partial class ThisUserMessageCell
    {
        public ThisUserMessageCell()
        {
            InitializeComponent();
        }

        protected override CachedImage UserImage => ProfilePicture;
    }
}
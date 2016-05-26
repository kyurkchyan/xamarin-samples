using Xamarin.Forms;

namespace VideoPlayerSample.Controls
{
    public class VideoPlayerContainer : Grid
    {
        #region Private fields and properties

        private FullscreenVideoPlayer _fullscreenVideoPlayer;
        private VideoPlayer _videoPlayer;
        private Image _overlayImage;

        #endregion

        #region Constructors

        public VideoPlayerContainer()
        {
            readdVideoPlayer();
            _videoPlayer.PlaybackStateChanged += VideoPlayerOnPlaybackStateChanged;
        }

        #endregion

        #region Public API

        public VideoPlayer Player => _videoPlayer;

        public static BindableProperty VideoPathProperty = BindableProperty.Create<VideoPlayerContainer, string>(o => o.VideoPath, null,
            propertyChanged: OnVideoPathChanged);

        public string VideoPath
        {
            get { return (string) GetValue(VideoPathProperty); }
            set { SetValue(VideoPathProperty, value); }
        }

        public void ToggleFullscreen()
        {
            //If the _fullscreenVideoPlayer is not null, it means it's already displayed
            if (_fullscreenVideoPlayer == null)
            {
                if (!string.IsNullOrWhiteSpace(VideoPath))
                {
                    _fullscreenVideoPlayer = new FullscreenVideoPlayer(VideoPath);
                    _fullscreenVideoPlayer.Disappearing += (sender, args) => {
                        //Destroy the full screen video player reference
                        _fullscreenVideoPlayer = null;
                        ////WE need to re-add the video player, because if we don't the video will not load
                        //readdVideoPlayer ();
                    };
                    Navigation.PushModalAsync(_fullscreenVideoPlayer, false);
                }
            }
        }

        #endregion

        #region Utility methods

        private static void OnVideoPathChanged(BindableObject bindable, string oldvalue, string newvalue)
        {
            var player = bindable as VideoPlayerContainer;
            player.Player.VideoPath = newvalue;
        }
        

        private void readdVideoPlayer()
        {

            _videoPlayer = new VideoPlayer();
            Children.Add(_videoPlayer);
            _overlayImage = new Image()
            {
                Source = "icon_play_overlay.png",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            Children.Add(_overlayImage);
        }

        private void VideoPlayerOnPlaybackStateChanged(object sender, bool isPlaying)
        {
            _overlayImage.IsVisible = !isPlaying;
        }

        #endregion
    }
}

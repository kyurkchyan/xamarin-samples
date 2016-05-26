using System;
using Xamarin.Forms;

namespace VideoPlayerSample.Controls
{
    public class VideoPlayer : View
    {
        #region Properties

        public Action TogglePlay { get; set; }
        public Action Stop { get; set; }
        public Action Play { get; set; }
        public Action HideFullScreenButton { get; set; }

        public event EventHandler<bool> PlaybackStateChanged;
        public event EventHandler PlaybackFinished;

        #region VideoPath

        public static BindableProperty VideoPathProperty = BindableProperty.Create<VideoPlayer, string>(o => o.VideoPath, "");

        public string VideoPath
        {
            get { return (string) GetValue(VideoPathProperty); }
            set { SetValue(VideoPathProperty, value); }
        }

        #endregion

        #region Show controls

        public static BindableProperty ShowControlsProperty = BindableProperty.Create<VideoPlayer, bool>(o => o.ShowControls, true);

        public bool ShowControls
        {
            get { return (bool) GetValue(ShowControlsProperty); }
            set { SetValue(ShowControlsProperty, value); }
        }

        #endregion


        #endregion

        #region Public API

        public void FirePlaybackStateChanged(object sender, bool isPlaying)
        {
            PlaybackStateChanged?.Invoke(sender, isPlaying);
        }


        public void FirePlaybackFinished(object sender, EventArgs args)
        {
            PlaybackFinished?.Invoke(sender, args);
        }

        #endregion

    }
}

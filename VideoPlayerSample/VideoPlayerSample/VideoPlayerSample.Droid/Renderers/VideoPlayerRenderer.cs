using System;
using System.ComponentModel;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using VideoPlayerSample.Controls;
using VideoPlayerSample.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(VideoPlayer), typeof(VideoPlayerRenderer))]
namespace VideoPlayerSample.Droid.Renderers
{
    public class VideoPlayerRenderer : ViewRenderer<VideoPlayer, Android.Views.View>
    {
        #region Private fields and properties

        private VideoPlayerView _player;
        private string _currentPath = null;

        #endregion

        #region Parent override

        protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayer> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                _player.OnPlay -= OnPlay;
                _player.OnPause -= OnPause;
                _player.Completion -= OnCompleted;
            }

            if (e.NewElement != null)
            {
                if (base.Control == null)
                {
                    LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                    var playerView = (FrameLayout)inflater.Inflate(Resource.Layout.video_view, null);

                    _player = playerView.FindViewById<VideoPlayerView>(Resource.Id.video_view);

                    _player.OnPlay += OnPlay;
                    _player.OnPause += OnPause;
                    _player.Completion += OnCompleted;
                    base.SetNativeControl(playerView);
                }
                e.NewElement.TogglePlay = TogglePlay;
                e.NewElement.Play = () => _player.Start();
                e.NewElement.Stop = () => _player.StopPlayback();
                //e.NewElement.HideFullScreenButton = hideFullScreenButton;
            }
            UpdateVideoPath();
            UpdateControls();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == VideoPlayer.VideoPathProperty.PropertyName)
            {
                UpdateVideoPath();
            }
            else if (e.PropertyName == VideoPlayer.ShowControlsProperty.PropertyName)
            {
                UpdateControls();
            }
        }

        #endregion

        #region Utility methods

        private void UpdateVideoPath()
        {
            if (_currentPath != Element.VideoPath)
            {
                _player?.SetVideoPath(Element.VideoPath);
                _currentPath = Element.VideoPath;
            }
        }

        private void UpdateControls()
        {
            if (Element.ShowControls)
            {
                MediaController mediaController = new MediaController(Context);
                mediaController.SetAnchorView(_player);
                _player.SetMediaController(mediaController);
            }
            else
            {
                _player.SetMediaController(null);
            }
        }

        private void TogglePlay()
        {
            if (_player == null)
                return;
            if (_player.IsPlaying)
                _player.Pause();
            else
                _player.Start();
        }

        private void OnPlay(object sender, EventArgs eventArgs)
        {
            Element.FirePlaybackStateChanged(this, true);
        }

        private void OnPause(object sender, EventArgs eventArgs)
        {
            Element.FirePlaybackStateChanged(this, false);
        }

        private void OnCompleted(object sender, EventArgs eventArgs)
        {
            Element.FirePlaybackFinished(this, EventArgs.Empty);
        }

        #endregion

    }

    #region VideoView

    public class VideoPlayerView : Android.Widget.VideoView
    {
        public VideoPlayerView(Context context)
            :base(context)
        {
            
        }

        public VideoPlayerView(IntPtr javaReference, JniHandleOwnership transfer)
            :base(javaReference, transfer)
        {
            
        }

        public VideoPlayerView(Context context, IAttributeSet attrs)
            :base(context, attrs)
        {
            
        }

        public event EventHandler<EventArgs> OnPause;
        public event EventHandler<EventArgs> OnPlay;

        public override void Pause()
        {
            base.Pause();
            OnPause?.Invoke(this, EventArgs.Empty);
        }

        public override void Start()
        {
            base.Start();
            OnPlay?.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion
}




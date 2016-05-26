using System;
using System.Collections.Generic;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using MediaPlayer;
using UIKit;
using VideoPlayerSample.Controls;
using VideoPlayerSample.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(VideoPlayer), typeof(VideoPlayerRenderer))]
namespace VideoPlayerSample.iOS.Renderers
{
    public class VideoPlayerRenderer : ViewRenderer<VideoPlayer, UIView>
    {
        #region Private fields and properties

        private MPMoviePlayerViewController _player;
        private List<NSObject> _observers = new List<NSObject> ();
        private UIButton _supposedFullscreenButton;

        #endregion

        #region Parent override

        protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayer> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (base.Control == null)
                {
                    _player = new MPMoviePlayerViewController();
                    _player.MoviePlayer.ShouldAutoplay = false;
                    _player.MoviePlayer.ScalingMode = MPMovieScalingMode.AspectFit;
                    _player.MoviePlayer.PrepareToPlay();

                    //_player.View.AddGestureRecognizer(new UITapGestureRecognizer(() => Element.FirePlayerTapped(this)));
                    base.SetNativeControl(_player.View);

                    //Add to root view controller as child
                    var rootVC = UIApplication.SharedApplication?.KeyWindow?.RootViewController;
                    if(rootVC == null)
                        return;
                    rootVC.AddChildViewController(_player);
                    _player.DidMoveToParentViewController(rootVC);

                    //Subscribe to necessary notifications
                    var center = NSNotificationCenter.DefaultCenter;
                    _observers.Add(center.AddObserver(MPMoviePlayerController.PlaybackStateDidChangeNotification, playbackStateChanged));
                    _observers.Add(center.AddObserver(MPMoviePlayerController.PlaybackDidFinishNotification, playbackFinished));
                }
                e.NewElement.TogglePlay = togglePlay;
                e.NewElement.Play = () => _player.MoviePlayer.Play();
                e.NewElement.Stop = () => _player.MoviePlayer.Stop();
                e.NewElement.HideFullScreenButton = hideFullScreenButton;
            }
            if(Element == null)
                return;
            
            updateVideoPath();
            updateControls();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == VideoPlayer.VideoPathProperty.PropertyName)
            {
                updateVideoPath();
            }
            else if (e.PropertyName == VideoPlayer.ShowControlsProperty.PropertyName)
            {
                updateControls();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                NSNotificationCenter.DefaultCenter.RemoveObservers(_observers);
                if (_player != null)
                {
                    _player.Dispose();
                    _player = null;
                }
            }
        }

        #endregion

        #region Player notifications

        private void playbackStateChanged(NSNotification notification)
        {
            if (_player.MoviePlayer.PlaybackState == MPMoviePlaybackState.Playing)
            {
                Element.FirePlaybackStateChanged(this, true);
            }
            else
            {
                Element.FirePlaybackStateChanged(this, false);
            }
        }

        private void playbackFinished(NSNotification notification)
        {

            var finishReason = (NSNumber)(notification.UserInfo[MPMoviePlayerController.PlaybackDidFinishReasonUserInfoKey]);
            if (finishReason.Int32Value == (int)MPMovieFinishReason.PlaybackEnded &&
                _player.MoviePlayer.Duration == _player.MoviePlayer.CurrentPlaybackTime)
            {
                Element.FirePlaybackFinished(this, new EventArgs());
            }
        }

        #endregion



        #region Utility methods

        private void updateVideoPath()
        {
            if (_player != null && Element != null)
            {
                _player.MoviePlayer.ContentUrl = !string.IsNullOrWhiteSpace(Element.VideoPath) ? NSUrl.FromFilename(Element.VideoPath) : null;
            }
        }

        private void updateControls()
        {
            if (_player != null && Element != null)
            {
                _player.MoviePlayer.ControlStyle = Element.ShowControls
                    ? MPMovieControlStyle.Embedded
                    : MPMovieControlStyle.None;
            }
        }


        private void togglePlay()
        {
            if (_player == null)
                return;
            if (_player.MoviePlayer.PlaybackState == MPMoviePlaybackState.Playing)
                _player.MoviePlayer.Pause();
            else
                _player.MoviePlayer.Play();
        }

        private void hideFullScreenButton ()
        {
            findFullScreenButton (_player.MoviePlayer.View);
            if (_supposedFullscreenButton != null)
                _supposedFullscreenButton.Hidden = true;
        }

        private CGPoint _maxPoint = new CGPoint(0, 0);
        private void findFullScreenButton(UIView parent)
        {
            foreach (var subview in parent.Subviews)
            {                
                if (subview is UIButton) {
                    if (subview.Frame.X > _maxPoint.X || subview.Frame.Y > _maxPoint.Y) {
                        _supposedFullscreenButton = (UIButton)subview;
                        _maxPoint = subview.Frame.Location;
                    }
                }
                findFullScreenButton(subview);
            }
        }

        #endregion


    }
}

using System;
using Xamarin.Forms;

namespace VideoPlayerSample.Controls
{
	public partial class FullscreenVideoPlayer : ContentPage
	{
	    private readonly string _path;

	    public FullscreenVideoPlayer (string path)
		{
	        InitializeComponent ();
            videoPlayer.VideoPath = path;
            videoPlayer.GestureRecognizers.Add(new TapGestureRecognizer(_ =>
            {
                videoPlayer.Player.TogglePlay();
            }));
	        videoPlayer.Player.PlaybackFinished += (sender, args) =>
	        {
	            Navigation.PopModalAsync();
	        };
		}

	 

	    protected override void OnAppearing()
	    {
	        base.OnAppearing();
            videoPlayer.Player.HideFullScreenButton?.Invoke ();
            videoPlayer.Player.TogglePlay?.Invoke();
        }

	    protected override void OnDisappearing()
	    {
	        base.OnDisappearing();
            videoPlayer.Player.Stop?.Invoke();
	    }

	    private void DoneButtonClicked(object sender, EventArgs e)
	    {
	        Navigation.PopModalAsync();
	    }
	}
}

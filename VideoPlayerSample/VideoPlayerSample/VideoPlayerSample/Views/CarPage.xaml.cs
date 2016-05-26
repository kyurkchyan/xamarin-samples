using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoPlayerSample.ViewModels;
using Xamarin.Forms;

namespace VideoPlayerSample.Views
{
    public partial class CarPage : ContentPage
    {
        public CarPage()
        {
            InitializeComponent();
            BindingContext = new CarViewModel();
            videoPlayer.Player.ShowControls = false;
            videoPlayer.GestureRecognizers.Add(new TapGestureRecognizer(g =>
            {
                videoPlayer.ToggleFullscreen();
            }));
        }
    }
}

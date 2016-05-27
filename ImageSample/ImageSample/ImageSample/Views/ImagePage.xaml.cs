using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageSample.ViewModels;
using Xamarin.Forms;

namespace ImageSample.Views
{
    public partial class ImagePage : ContentPage
    {
        public ImagePage()
        {
            InitializeComponent();
            BindingContext = new ImageViewModel();
        }
    }
}

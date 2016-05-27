using Base64Sample.ViewModels;
using Xamarin.Forms;

namespace Base64Sample.Views
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

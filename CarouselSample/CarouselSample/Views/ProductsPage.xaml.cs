using CarouselSample.ViewModels;
using Xamarin.Forms;

namespace CarouselSample.Views
{
	public partial class CarsPage
	{
		public CarsPage()
		{
			InitializeComponent();
            BindingContext = new CarsViewModel();
		}
	}
}


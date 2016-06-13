using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabSample.ViewModels;
using Xamarin.Forms;

namespace TabSample.Views
{
    public partial class TabSamplePage : ContentPage
    {
        public TabSamplePage()
        {
            InitializeComponent();
            BindingContext = new CarsViewModel();
        }
    }
}

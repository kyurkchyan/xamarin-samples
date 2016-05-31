using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace SvgSample.Views
{
    public partial class SvgPage : ContentPage
    {
        public SvgPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            webView.LoadFromContent("HTML/guage.html");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FontSample.ViewModels;
using Xamarin.Forms;

namespace FontSample.Views
{
    public partial class FontDemoPage
    {
        public FontDemoPage()
        {
            InitializeComponent();
            BindingContext = new FontDemoViewModel();
        }
    }
}

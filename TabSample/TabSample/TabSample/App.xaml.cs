using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabSample.Views;
using Xamarin.Forms;

namespace TabSample
{
    public partial class App : Application
    {
        private static App _instance;
        public App()
        {
            InitializeComponent();
            _instance = this;
            // The root page of your application
            MainPage = new NavigationPage(new TabSamplePage());
        }

        public static App Instance => _instance;
        
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FontSample.Views;
using Xamarin.Forms;

namespace FontSample
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application
            MainPage = new NavigationPage(new FontDemoPage());
        }

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

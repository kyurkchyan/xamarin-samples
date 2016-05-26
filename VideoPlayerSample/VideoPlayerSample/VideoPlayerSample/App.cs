using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoPlayerSample.Views;
using Xamarin.Forms;

namespace VideoPlayerSample
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application
            MainPage = new NavigationPage(new CarPage());
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

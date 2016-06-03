using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewButtonSample.Views;
using Xamarin.Forms;

namespace ViewButtonSample
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application
            MainPage = new NavigationPage(new SamplePage());
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntryExSample.Controls;
using Xamarin.Forms;

namespace EntryExSample
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application
            MainPage = new ContentPage
            {
                BackgroundColor = Color.White,
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Padding = 10,
                    Children = {
                        new EntryEx() {
                            BorderWidth = 2,
                            BorderRadius = 5,
                            BorderColor = Color.Gray,
                            PlaceholderColor = Color.Navy,
                            Placeholder = "Sample placeholder",
                            HeightRequest = 60,
                            LeftPadding = 10,
                            RightPadding = 10,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand
                        }
                    }
                }
            };
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

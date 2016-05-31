using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using XLabs.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;
using XLabs.Serialization;

namespace SvgSample.Droid
{
    [Activity(Label = "SvgSample", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : XFormsApplicationDroid
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            

            if (!Resolver.IsSet)
            {
                var container = new SimpleContainer();
                container.Register<IDevice>(t => AndroidDevice.CurrentDevice);
                container.Register<IDisplay>(t => t.Resolve<IDevice>().Display);
                container.Register<INetwork>(t => t.Resolve<IDevice>().Network);

                Resolver.SetResolver(container.GetResolver());
            }

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}


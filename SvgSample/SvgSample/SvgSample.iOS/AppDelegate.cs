using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using XLabs.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;

namespace SvgSample.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : XFormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            HybridWebViewRenderer.CopyBundleDirectory("HTML");
            if (!Resolver.IsSet)
            {
                var container = new SimpleContainer();
                container.Register<IDevice>(t => AppleDevice.CurrentDevice);
                container.Register<IDisplay>(t => t.Resolve<IDevice>().Display);
                container.Register<INetwork>(t => t.Resolve<IDevice>().Network);

                Resolver.SetResolver(container.GetResolver());
            }

            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}

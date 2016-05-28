using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CoreGraphics;
using CoreText;
using FontSample.iOS.Services;
using FontSample.Model;
using FontSample.Services;
using Foundation;
using PCLStorage;

[assembly: Xamarin.Forms.Dependency(typeof(FontService))]
namespace FontSample.iOS.Services
{
    public class FontService : BaseFontService
    {
        protected override Task<IFolder> GetCacheFolder()
        {
            return FileSystem.Current.LocalStorage.GetFolderAsync("Caches");
        }

        //We need to register fonts with iOS runtime
        //Take a look at following article for more details
        //https://marco.org/2012/12/21/ios-dynamic-font-loading
        protected override void InstallFonts(IEnumerable<CustomFont> fonts)
        {
            NSError error = null;
            foreach (var font in fonts)
            {
                var fontData = NSData.FromArray(font.Data);
                using (CGDataProvider provider = new CGDataProvider(fontData))
                {
                    using (CGFont nativeFont = CGFont.CreateFromProvider(provider))
                    {
                        CTFontManager.RegisterGraphicsFont(nativeFont, out error);
                        if (error != null)
                        {
                            Console.WriteLine(error);
                        }
                    }
                }
            }
        }
    }
}

using System;
using Android.Content;
using Android.Graphics;
using FontSample.Services;
using Java.IO;
using PCLStorage;
using Xamarin.Forms;
using Console = System.Console;

namespace FontSample.Droid.Toolbox
{
    public static class FontToolbox
    {
        private static readonly IFontService _fontService;

        static FontToolbox()
        {
            _fontService = DependencyService.Get<IFontService>();
        }

        public static Typeface TryGetFont(this Context context, string name, FontAttributes attr)
        {
            if (string.IsNullOrEmpty(name))
                return Typeface.Default;

            //Try to get font file based on font name
            string filename = null;
            _fontService.Fonts?.TryGetValue(name, out filename);

            //get folder containing fonts
            var fontFolder = _fontService?.FontFolder?.Path;

            //If we haven't found font in custom fonts, try to find it in system defaults
            if (filename == null || fontFolder == null)
                return Typeface.Create(name, attr.ToAndroid());

            //Create typeface from that font file
            var path = PortablePath.Combine(fontFolder, filename);
            var font = Typeface.CreateFromFile(new File(path));
            return font;
        }

        public static TypefaceStyle ToAndroid(this FontAttributes attr)
        {
            if ((attr & FontAttributes.Italic) == FontAttributes.Italic)
                return TypefaceStyle.Italic;
            if ((attr & FontAttributes.Bold) == FontAttributes.Bold)
                return TypefaceStyle.Bold;
            return TypefaceStyle.Normal;
        }
    }
}
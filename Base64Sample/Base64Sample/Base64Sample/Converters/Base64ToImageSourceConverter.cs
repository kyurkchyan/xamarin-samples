using System;
using System.Globalization;
using System.IO;
using Xamarin.Forms;

namespace Base64Sample.Converters
{
    public class Base64ToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Try to read value as string and convert it to ImageSource using streams
            //If the operation fails for some reason, return null.
            try
            {
                var image = value.ToString();
                if (image == null)
                    return null;
                byte[] data = System.Convert.FromBase64String(image);

                var imageSource = ImageSource.FromStream(() => new MemoryStream(data));
                return imageSource;
            }
            catch
            {
                return null;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabSample.Controls;
using Xamarin.Forms;

namespace TabSample.Converters
{
    public class TabTitleStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isSelected = value as bool?;
            if (isSelected == true)
            {
                return Application.Current.Resources["SelectedTabTitleStyle"];
            }
            return Application.Current.Resources["TabTitleStyle"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

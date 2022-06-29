using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Library.GUI_App.Utilities
{
    public class DecimalConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Decimal.Parse(value.ToString());
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }
    }
}

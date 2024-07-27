using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ChatApp.Converters;

public class NullImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Uri uri && !string.IsNullOrEmpty(uri.OriginalString))
        {
            return uri.OriginalString;
        }
        return new Uri("/ChatApp;component/Assets/Images/6.jpg", UriKind.Relative);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
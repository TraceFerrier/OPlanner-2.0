using System;
using System.Windows.Data;
using System.Windows.Media;

namespace PlannerNameSpace.View.Converters
{
    public class ChangesToSaveColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int changesToSave = (int)value;
            if (changesToSave > 0)
            {
                var brushConverter = new BrushConverter();
                return (Brush)brushConverter.ConvertFrom(Constants.ChangesToSaveColor);
            }
            else
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

}

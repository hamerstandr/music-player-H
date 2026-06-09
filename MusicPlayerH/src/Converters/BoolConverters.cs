using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MusicPlayerH.Converters
{
    public class BoolToPlayPauseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isPlaying)
                return isPlaying ? "⏸️" : "▶️";
            return "▶️";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isConnected)
                return isConnected 
                    ? new SolidColorBrush(Color.FromRgb(29, 185, 84)) // Connected (Green)
                    : new SolidColorBrush(Color.FromRgb(255, 82, 82)); // Disconnected (Red)
            return new SolidColorBrush(Color.FromRgb(255, 82, 82));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

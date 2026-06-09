using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MusicPlayerH.Converters
{
    /// <summary>
    /// تبدیل وضعیت اتصال به رنگ
    /// </summary>
    public class BoolToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isConnected)
            {
                return isConnected ? new SolidColorBrush(Color.FromRgb(29, 185, 84)) : new SolidColorBrush(Colors.Gray);
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// تبدیل وضعیت پخش به آیکون Play/Pause
    /// </summary>
    public class BoolToPlayPauseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isPlaying)
            {
                return isPlaying ? "⏸️" : "▶️";
            }
            return "▶️";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

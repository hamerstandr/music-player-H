using System.Windows;

namespace MusicPlayerH
{
    /// <summary>
    /// پنجره اصلی برنامه پخش موسیقی
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // تنظیم DataContext
            var viewModel = new ViewModels.MainViewModel();
            DataContext = viewModel;
            
            // ثبت رویداد بسته شدن برای پاک‌سازی منابع
            Closed += (s, e) =>
            {
                if (viewModel is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            };
        }
    }
}

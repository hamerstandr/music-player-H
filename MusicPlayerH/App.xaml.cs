using System.Windows;

namespace MusicPlayerH
{
    /// <summary>
    /// کلاس اصلی برنامه
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // تنظیمات اولیه برنامه
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            
            // پاک‌سازی منابع
        }
    }
}

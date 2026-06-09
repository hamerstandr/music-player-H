using System.Windows;
using MusicPlayerH.Services;

namespace MusicPlayerH
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Initialize TrafficWatch plugin client (non-blocking)
            // It will connect if TrafficWatch is running, otherwise continue normally
            Task.Run(async () =>
            {
                await Task.Delay(1000); // Small delay to ensure UI is ready
                var pluginClient = TrafficWatchPluginClient.Instance;
                await pluginClient.InitializeAsync();
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Cleanup
            TrafficWatchPluginClient.Instance.Dispose();
            base.OnExit(e);
        }
    }
}

using System;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerH.Services
{
    public class TrafficWatchPluginClient : IDisposable
    {
        private static TrafficWatchPluginClient? _instance;
        private NamedPipeClientStream? _pipeClient;
        private bool _isConnected;
        private readonly string _pipeName = "TrafficWatchPluginPipe";
        private readonly string _pluginId = Guid.NewGuid().ToString("N")[..8];

        public static TrafficWatchPluginClient Instance => _instance ??= new TrafficWatchPluginClient();

        public event Action<bool>? ConnectionStateChanged;

        public bool IsConnected => _isConnected;

        private TrafficWatchPluginClient() { }

        public async Task InitializeAsync()
        {
            try
            {
                await ConnectAsync();
                if (_isConnected)
                {
                    await RegisterAsync();
                }
            }
            catch
            {
                // TrafficWatch is not running, continue normally
                _isConnected = false;
                ConnectionStateChanged?.Invoke(false);
            }
        }

        private async Task ConnectAsync()
        {
            try
            {
                _pipeClient = new NamedPipeClientStream(
                    ".", 
                    _pipeName, 
                    PipeDirection.Out, 
                    PipeOptions.Asynchronous);

                await _pipeClient.ConnectAsync(2000); // 2 second timeout

                if (_pipeClient.IsConnected)
                {
                    _isConnected = true;
                    ConnectionStateChanged?.Invoke(true);
                }
            }
            catch
            {
                _isConnected = false;
                _pipeClient?.Dispose();
                _pipeClient = null;
                throw;
            }
        }

        private async Task RegisterAsync()
        {
            if (!_isConnected || _pipeClient == null) return;

            var registerMessage = new
            {
                action = "register",
                name = "Music Player H",
                version = "2.0.0",
                icon = "🎵"
            };

            await SendMessageAsync(registerMessage);
        }

        public async Task SendDataAsync(string jsonData)
        {
            if (!_isConnected || _pipeClient == null) return;

            try
            {
                var bytes = Encoding.UTF8.GetBytes(jsonData + "\n");
                await _pipeClient.WriteAsync(bytes, 0, bytes.Length);
                await _pipeClient.FlushAsync();
            }
            catch
            {
                // Connection lost, try to reconnect
                _isConnected = false;
                ConnectionStateChanged?.Invoke(false);
                _ = ReconnectAsync();
            }
        }

        private async Task SendMessageAsync(object message)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(message);
            await SendDataAsync(json);
        }

        private async Task ReconnectAsync()
        {
            try
            {
                _pipeClient?.Dispose();
                _pipeClient = null;
                
                await Task.Delay(5000); // Wait 5 seconds before retry
                await ConnectAsync();
                
                if (_isConnected)
                {
                    await RegisterAsync();
                }
            }
            catch
            {
                // Still can't connect, will retry later
            }
        }

        public void Dispose()
        {
            _isConnected = false;
            _pipeClient?.Dispose();
            _pipeClient = null;
        }
    }
}

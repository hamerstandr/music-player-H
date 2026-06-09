using System;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MusicPlayerH.Services
{
    /// <summary>
    /// سرویس ارتباط با TrafficWatch از طریق Named Pipe
    /// </summary>
    public class TrafficWatchPluginClient : IDisposable
    {
        private const string PipeName = "TrafficWatchPluginPipe";
        private NamedPipeClientStream? _pipeClient;
        private CancellationTokenSource? _heartbeatCts;
        private Task? _heartbeatTask;
        private bool _isConnected;
        private string? _pluginId;
        private readonly object _lock = new();

        public bool IsConnected => _isConnected;
        public string? PluginId => _pluginId;

        public event EventHandler<string>? OnMessageReceived;
        public event EventHandler? OnConnected;
        public event EventHandler? OnDisconnected;

        public async Task ConnectAsync()
        {
            try
            {
                _pipeClient = new NamedPipeClientStream(
                    ".", 
                    PipeName, 
                    PipeDirection.InOut, 
                    PipeOptions.Asynchronous);

                await _pipeClient.ConnectAsync(5000);
                
                if (_pipeClient.IsConnected)
                {
                    _isConnected = true;
                    OnConnected?.Invoke(this, EventArgs.Empty);
                    
                    // شروع Heartbeat
                    StartHeartbeat();
                    
                    // ثبت نام خودکار
                    await RegisterAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TrafficWatch] خطا در اتصال: {ex.Message}");
                _isConnected = false;
            }
        }

        private async Task RegisterAsync()
        {
            var registerMessage = new
            {
                action = "register",
                name = "Music Player H",
                version = "2.0.0",
                icon = "🎵"
            };

            await SendMessageAsync(registerMessage);
        }

        private void StartHeartbeat()
        {
            _heartbeatCts = new CancellationTokenSource();
            _heartbeatTask = Task.Run(async () =>
            {
                while (!_heartbeatCts.Token.IsCancellationRequested && _isConnected)
                {
                    try
                    {
                        await Task.Delay(30000, _heartbeatCts.Token);
                        
                        if (_isConnected)
                        {
                            var heartbeatMessage = new { action = "heartbeat" };
                            await SendMessageAsync(heartbeatMessage);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[TrafficWatch] خطا در ارسال Heartbeat: {ex.Message}");
                    }
                }
            });
        }

        public async Task SendStreamDataAsync(string title, string artist, string album, 
            string duration, int progress, bool isPlaying)
        {
            if (!_isConnected || _pipeClient == null)
                return;

            var streamData = new
            {
                action = "stream_data",
                timestamp = DateTime.UtcNow.ToString("o"),
                payload = new
                {
                    type = "now_playing",
                    title,
                    artist,
                    album,
                    duration,
                    progress,
                    isPlaying
                }
            };

            await SendMessageAsync(streamData);
        }

        private async Task SendMessageAsync(object message)
        {
            if (_pipeClient == null || !_pipeClient.IsConnected)
                return;

            lock (_lock)
            {
                try
                {
                    var json = JsonSerializer.Serialize(message);
                    var bytes = Encoding.UTF8.GetBytes(json + "\n");
                    _pipeClient.Write(bytes, 0, bytes.Length);
                    await _pipeClient.FlushAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[TrafficWatch] خطا در ارسال پیام: {ex.Message}");
                    HandleDisconnection();
                }
            }
        }

        public async Task StartListeningAsync()
        {
            if (_pipeClient == null)
                return;

            var buffer = new byte[4096];
            
            try
            {
                while (_isConnected && _pipeClient.IsConnected)
                {
                    var bytesRead = await _pipeClient.ReadAsync(buffer, 0, buffer.Length);
                    
                    if (bytesRead > 0)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                        
                        if (!string.IsNullOrEmpty(message))
                        {
                            try
                            {
                                using var doc = JsonDocument.Parse(message);
                                
                                if (doc.RootElement.TryGetProperty("action", out var actionElement))
                                {
                                    var action = actionElement.GetString();
                                    
                                    if (action == "registered" && 
                                        doc.RootElement.TryGetProperty("id", out var idElement))
                                    {
                                        _pluginId = idElement.GetString();
                                        Console.WriteLine($"[TrafficWatch] افزونه ثبت شد. شناسه: {_pluginId}");
                                    }
                                }
                                
                                OnMessageReceived?.Invoke(this, message);
                            }
                            catch (JsonException)
                            {
                                // نادیده گرفتن پیام‌های نامعتبر
                            }
                        }
                    }
                }
            }
            catch (IOException)
            {
                HandleDisconnection();
            }
        }

        private void HandleDisconnection()
        {
            if (_isConnected)
            {
                _isConnected = false;
                _pluginId = null;
                OnDisconnected?.Invoke(this, EventArgs.Empty);
                Console.WriteLine("[TrafficWatch] اتصال قطع شد.");
            }
        }

        public void Disconnect()
        {
            _heartbeatCts?.Cancel();
            _pipeClient?.Dispose();
            _isConnected = false;
            _pluginId = null;
        }

        public void Dispose()
        {
            Disconnect();
            _heartbeatCts?.Dispose();
        }
    }
}

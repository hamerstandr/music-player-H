using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MusicPlayerH.Services.PluginSystem
{
    /// <summary>
    /// نمونه کلاینت برای اتصال به سرور Named Pipe TrafficWatch
    /// این کلاس می‌تواند در افزونه‌های شخص ثالث استفاده شود
    /// </summary>
    public class PluginClient : IDisposable
    {
        private const string PIPE_NAME = "TrafficWatchPluginPipe";
        private NamedPipeClientStream _pipeStream;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _readTask;
        private bool _isConnected;
        private string _clientId;

        /// <summary>
        /// رویداد دریافت پاسخ از سرور
        /// </summary>
        public event EventHandler<PluginResponseEventArgs> OnServerResponse;
        
        /// <summary>
        /// رویداد قطع اتصال
        /// </summary>
        public event EventHandler OnDisconnected;

        public PluginClient()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// اتصال به سرور Named Pipe
        /// </summary>
        public async Task<bool> ConnectAsync(int timeoutMs = 5000)
        {
            if (_isConnected) return true;

            try
            {
                _pipeStream = new NamedPipeClientStream(
                    ".", 
                    PIPE_NAME, 
                    PipeDirection.InOut, 
                    PipeOptions.Asynchronous
                );

                await _pipeStream.ConnectAsync(timeoutMs, _cancellationTokenSource.Token);
                
                if (!_pipeStream.IsConnected)
                    return false;

                _isConnected = true;
                _readTask = Task.Run(() => ReadResponsesAsync(_cancellationTokenSource.Token));
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to connect to pipe: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// ثبت نام در سرور
        /// </summary>
        public async Task<string> RegisterAsync(string name, string version, string icon = "📦")
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to server");

            var request = new
            {
                action = "register",
                name = name,
                version = version,
                icon = icon
            };

            string json = JsonSerializer.Serialize(request);
            await SendMessageAsync(json);

            // منتظر پاسخ ثبت نام می‌مانیم
            var tcs = new TaskCompletionSource<string>();
            EventHandler<PluginResponseEventArgs> handler = null;
            handler = (s, e) =>
            {
                if (e.Response.RootElement.TryGetProperty("action", out var action))
                {
                    if (action.GetString() == "registered")
                    {
                        if (e.Response.RootElement.TryGetProperty("id", out var idElement))
                        {
                            _clientId = idElement.GetString();
                            tcs.SetResult(_clientId);
                        }
                    }
                }
            };

            OnServerResponse += handler;
            
            try
            {
                return await tcs.Task.WaitAsync(TimeSpan.FromSeconds(5));
            }
            finally
            {
                OnServerResponse -= handler;
            }
        }

        /// <summary>
        /// ارسال داده استریم به سرور
        /// </summary>
        public async Task SendStreamDataAsync(object payload)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to server");

            var message = new
            {
                action = "stream_data",
                timestamp = DateTime.UtcNow.ToString("o"),
                payload = payload
            };

            string json = JsonSerializer.Serialize(message);
            await SendMessageAsync(json);
        }

        /// <summary>
        /// ارسال Heartbeat برای حفظ اتصال
        /// </summary>
        public async Task SendHeartbeatAsync()
        {
            if (!_isConnected)
                return;

            var message = new { action = "heartbeat" };
            string json = JsonSerializer.Serialize(message);
            await SendMessageAsync(json);
        }

        /// <summary>
        /// ارسال پیام خام به سرور
        /// </summary>
        private async Task SendMessageAsync(string message)
        {
            if (!_isConnected || _pipeStream == null)
                return;

            try
            {
                using var writer = new StreamWriter(_pipeStream, Encoding.UTF8, 4096, leaveOpen: true)
                {
                    AutoFlush = true
                };
                await writer.WriteLineAsync(message);
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending message: {ex.Message}");
                _isConnected = false;
                OnDisconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// خواندن پاسخ‌ها از سرور
        /// </summary>
        private async Task ReadResponsesAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var reader = new StreamReader(_pipeStream, Encoding.UTF8, false, 4096, leaveOpen: true);

                while (!cancellationToken.IsCancellationRequested && _isConnected)
                {
                    string line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line))
                    {
                        _isConnected = false;
                        break;
                    }

                    try
                    {
                        using var response = JsonDocument.Parse(line);
                        OnServerResponse?.Invoke(this, new PluginResponseEventArgs
                        {
                            Response = response
                        });
                    }
                    catch (JsonException ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Invalid JSON response: {ex.Message}");
                    }
                }
            }
            catch (IOException)
            {
                _isConnected = false;
            }
            finally
            {
                OnDisconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// قطع اتصال از سرور
        /// </summary>
        public async Task DisconnectAsync()
        {
            if (!_isConnected) return;

            _isConnected = false;
            _cancellationTokenSource?.Cancel();
            
            if (_readTask != null)
            {
                try
                {
                    await _readTask;
                }
                catch { }
            }

            _pipeStream?.Dispose();
            _pipeStream = null;
        }

        public bool IsConnected => _isConnected;
        public string ClientId => _clientId;

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
            _pipeStream?.Dispose();
        }
    }

    /// <summary>
    /// آرگومان‌های رویداد پاسخ سرور
    /// </summary>
    public class PluginResponseEventArgs : EventArgs
    {
        public JsonDocument Response { get; set; }
    }

    /// <summary>
    /// مثال استفاده از PluginClient برای پخش کننده موسیقی
    /// </summary>
    public class MusicStreamerPlugin
    {
        private readonly PluginClient _client;
        private System.Timers.Timer _heartbeatTimer;
        private bool _isRunning;

        public MusicStreamerPlugin()
        {
            _client = new PluginClient();
            _client.OnDisconnected += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("Disconnected from TrafficWatch server");
                Stop();
            };
        }

        /// <summary>
        /// شروع اتصال و ارسال داده
        /// </summary>
        public async Task StartAsync()
        {
            if (_isRunning) return;

            // اتصال به سرور
            bool connected = await _client.ConnectAsync();
            if (!connected)
            {
                System.Diagnostics.Debug.WriteLine("Failed to connect to TrafficWatch server");
                return;
            }

            // ثبت نام
            string clientId = await _client.RegisterAsync(
                "Music Player Plugin",
                "2.0.0",
                "🎵"
            );

            System.Diagnostics.Debug.WriteLine($"Registered with ID: {clientId}");

            // تنظیم تایمر Heartbeat (هر 30 ثانیه)
            _heartbeatTimer = new System.Timers.Timer(30000);
            _heartbeatTimer.Elapsed += async (s, e) =>
            {
                await _client.SendHeartbeatAsync();
            };
            _heartbeatTimer.Start();

            _isRunning = true;

            // شبیه‌سازی ارسال داده موسیقی
            await SimulateMusicStreamingAsync();
        }

        /// <summary>
        /// توقف پلاگین
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
            _heartbeatTimer?.Stop();
            _heartbeatTimer?.Dispose();
            _ = _client.DisconnectAsync();
        }

        /// <summary>
        /// شبیه‌سازی ارسال داده موسیقی
        /// </summary>
        private async Task SimulateMusicStreamingAsync()
        {
            // در برنامه واقعی، اینجا اطلاعات آهنگ در حال پخش خوانده می‌شود
            var nowPlayingData = new
            {
                type = "now_playing",
                title = "Bohemian Rhapsody",
                artist = "Queen",
                album = "A Night at the Opera",
                duration = "5:55",
                progress = 45,
                isPlaying = true
            };

            await _client.SendStreamDataAsync(nowPlayingData);
            
            System.Diagnostics.Debug.WriteLine("Sent music data to TrafficWatch");
        }

        /// <summary>
        /// بروزرسانی اطلاعات آهنگ در حال پخش
        /// </summary>
        public async Task UpdateNowPlayingAsync(string title, string artist, string album, int progress, bool isPlaying)
        {
            var nowPlayingData = new
            {
                type = "now_playing",
                title = title,
                artist = artist,
                album = album,
                progress = progress,
                isPlaying = isPlaying
            };

            await _client.SendStreamDataAsync(nowPlayingData);
        }
    }
}

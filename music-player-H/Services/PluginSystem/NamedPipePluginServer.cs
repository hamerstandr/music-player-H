using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MusicPlayerH.Services.PluginSystem
{
    /// <summary>
    /// سرور Named Pipe برای ارتباط با افزونه‌های TrafficWatch
    /// </summary>
    public class NamedPipePluginServer : IDisposable
    {
        private const string PIPE_NAME = "TrafficWatchPluginPipe";
        private NamedPipeServerStream _serverStream;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _listenTask;
        private bool _isRunning;
        
        private readonly Dictionary<string, PluginClientInfo> _connectedClients;
        private readonly object _clientsLock = new object();

        /// <summary>
        /// رویداد دریافت داده از افزونه‌ها
        /// </summary>
        public event EventHandler<PluginDataReceivedEventArgs> OnDataReceived;
        
        /// <summary>
        /// رویداد اتصال کلاینت جدید
        /// </summary>
        public event EventHandler<PluginClientConnectedEventArgs> OnClientConnected;
        
        /// <summary>
        /// رویداد قطع اتصال کلاینت
        /// </summary>
        public event EventHandler<PluginClientDisconnectedEventArgs> OnClientDisconnected;

        public NamedPipePluginServer()
        {
            _connectedClients = new Dictionary<string, PluginClientInfo>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// شروع گوش دادن به Named Pipe
        /// </summary>
        public async Task StartAsync()
        {
            if (_isRunning) return;

            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();
            
            _listenTask = Task.Run(() => ListenForConnectionsAsync(_cancellationTokenSource.Token));
            
            await Task.CompletedTask;
        }

        /// <summary>
        /// گوش دادن برای اتصالات جدید
        /// </summary>
        private async Task ListenForConnectionsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _serverStream = new NamedPipeServerStream(
                        PIPE_NAME,
                        PipeDirection.InOut,
                        NamedPipeServerStream.MaxAllowedServerInstances,
                        PipeTransmissionMode.Message,
                        PipeOptions.Asynchronous
                    );

                    await _serverStream.WaitForConnectionAsync(cancellationToken);
                    
                    // پردازش کلاینت متصل شده در تسک جداگانه
                    _ = HandleClientAsync(_serverStream, cancellationToken);
                    
                    // ایجاد سرور جدید برای کلاینت بعدی
                    _serverStream = null;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in pipe server: {ex.Message}");
                    await Task.Delay(100, cancellationToken);
                }
            }
        }

        /// <summary>
        /// مدیریت ارتباط با یک کلاینت
        /// </summary>
        private async Task HandleClientAsync(NamedPipeServerStream stream, CancellationToken cancellationToken)
        {
            var clientInfo = new PluginClientInfo
            {
                ConnectionTime = DateTime.Now,
                LastHeartbeat = DateTime.Now
            };
            
            string clientId = Guid.NewGuid().ToString("N")[..8];
            
            try
            {
                lock (_clientsLock)
                {
                    _connectedClients[clientId] = clientInfo;
                }

                using var reader = new StreamReader(stream, Encoding.UTF8, false, 4096, leaveOpen: true);
                using var writer = new StreamWriter(stream, Encoding.UTF8, 4096, leaveOpen: true) { AutoFlush = true };

                OnClientConnected?.Invoke(this, new PluginClientConnectedEventArgs
                {
                    ClientId = clientId,
                    ConnectionTime = clientInfo.ConnectionTime
                });

                while (!cancellationToken.IsCancellationRequested && stream.IsConnected)
                {
                    string line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line))
                    {
                        // قطع اتصال
                        break;
                    }

                    await ProcessMessageAsync(clientId, line, writer, clientInfo);
                }
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine($"IO Error with client {clientId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling client {clientId}: {ex.Message}");
            }
            finally
            {
                // قطع اتصال و حذف کلاینت
                lock (_clientsLock)
                {
                    _connectedClients.Remove(clientId);
                }
                
                try
                {
                    stream?.Dispose();
                }
                catch { }

                OnClientDisconnected?.Invoke(this, new PluginClientDisconnectedEventArgs
                {
                    ClientId = clientId
                });
            }
        }

        /// <summary>
        /// پردازش پیام دریافتی از کلاینت
        /// </summary>
        private async Task ProcessMessageAsync(string clientId, string message, StreamWriter writer, PluginClientInfo clientInfo)
        {
            try
            {
                using var doc = JsonDocument.Parse(message);
                var root = doc.RootElement;
                
                if (!root.TryGetProperty("action", out var actionElement))
                    return;

                string action = actionElement.GetString();
                
                switch (action?.ToLower())
                {
                    case "register":
                        await HandleRegisterAsync(clientId, root, writer, clientInfo);
                        break;
                        
                    case "stream_data":
                        await HandleStreamDataAsync(clientId, root);
                        break;
                        
                    case "heartbeat":
                        clientInfo.LastHeartbeat = DateTime.Now;
                        break;
                }
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"JSON parse error from {clientId}: {ex.Message}");
            }
        }

        /// <summary>
        /// پردازش درخواست ثبت نام
        /// </summary>
        private async Task HandleRegisterAsync(string clientId, JsonElement root, StreamWriter writer, PluginClientInfo clientInfo)
        {
            string name = root.GetProperty("name").GetString() ?? "Unknown Plugin";
            string version = root.GetProperty("version").GetString() ?? "1.0.0";
            string icon = root.GetProperty("icon").GetString() ?? "📦";

            clientInfo.Name = name;
            clientInfo.Version = version;
            clientInfo.Icon = icon;

            // ارسال پاسخ ثبت نام موفق
            var response = new
            {
                action = "registered",
                id = clientId,
                status = "success"
            };

            await writer.WriteLineAsync(JsonSerializer.Serialize(response));
            
            System.Diagnostics.Debug.WriteLine($"Plugin registered: {name} v{version} (ID: {clientId})");
        }

        /// <summary>
        /// پردازش داده استریم
        /// </summary>
        private async Task HandleStreamDataAsync(string clientId, JsonElement root)
        {
            // استخراج payload
            if (root.TryGetProperty("payload", out var payloadElement))
            {
                string payloadJson = payloadElement.GetRawText();
                
                OnDataReceived?.Invoke(this, new PluginDataReceivedEventArgs
                {
                    AddonId = clientId,
                    Data = payloadJson,
                    Timestamp = DateTime.Now
                });
            }
            
            await Task.CompletedTask;
        }

        /// <summary>
        /// توقف سرور
        /// </summary>
        public async Task StopAsync()
        {
            if (!_isRunning) return;

            _isRunning = false;
            _cancellationTokenSource?.Cancel();
            
            if (_listenTask != null)
            {
                try
                {
                    await _listenTask;
                }
                catch { }
            }

            _serverStream?.Dispose();
            _serverStream = null;
            
            lock (_clientsLock)
            {
                _connectedClients.Clear();
            }
        }

        /// <summary>
        /// دریافت لیست کلاینت‌های متصل
        /// </summary>
        public List<PluginClientInfo> GetConnectedClients()
        {
            lock (_clientsLock)
            {
                return new List<PluginClientInfo>(_connectedClients.Values);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
            _serverStream?.Dispose();
        }
    }

    /// <summary>
    /// اطلاعات کلاینت متصل
    /// </summary>
    public class PluginClientInfo
    {
        public string Name { get; set; } = "Unknown";
        public string Version { get; set; } = "1.0.0";
        public string Icon { get; set; } = "📦";
        public DateTime ConnectionTime { get; set; }
        public DateTime LastHeartbeat { get; set; }
    }

    /// <summary>
    /// آرگومان‌های رویداد دریافت داده
    /// </summary>
    public class PluginDataReceivedEventArgs : EventArgs
    {
        public string AddonId { get; set; }
        public string Data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// آرگومان‌های رویداد اتصال کلاینت
    /// </summary>
    public class PluginClientConnectedEventArgs : EventArgs
    {
        public string ClientId { get; set; }
        public DateTime ConnectionTime { get; set; }
    }

    /// <summary>
    /// آرگومان‌های رویداد قطع اتصال کلاینت
    /// </summary>
    public class PluginClientDisconnectedEventArgs : EventArgs
    {
        public string ClientId { get; set; }
    }
}

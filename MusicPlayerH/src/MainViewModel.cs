using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicPlayerH.Services;

namespace MusicPlayerH.ViewModels
{
    /// <summary>
    /// مدل نمایشی اصلی برنامه پخش موسیقی
    /// </summary>
    public class MainViewModel : BaseViewModel, IDisposable
    {
        private readonly TrafficWatchPluginClient _trafficWatchClient;
        
        private string _currentTitle = "عنوان آهنگ";
        private string _currentArtist = "هنرمند";
        private string _currentAlbum = "آلبوم";
        private string _currentDuration = "0:00";
        private int _currentProgress;
        private bool _isPlaying;
        private string _connectionStatus = "در حال اتصال...";
        private bool _isConnected;

        public ObservableCollection<MusicTrack> Playlist { get; } = new();
        
        private MusicTrack? _selectedTrack;
        public MusicTrack? SelectedTrack
        {
            get => _selectedTrack;
            set
            {
                _selectedTrack = value;
                OnPropertyChanged();
                if (value != null)
                {
                    LoadTrack(value);
                }
            }
        }

        public string CurrentTitle
        {
            get => _currentTitle;
            set { _currentTitle = value; OnPropertyChanged(); }
        }

        public string CurrentArtist
        {
            get => _currentArtist;
            set { _currentArtist = value; OnPropertyChanged(); }
        }

        public string CurrentAlbum
        {
            get => _currentAlbum;
            set { _currentAlbum = value; OnPropertyChanged(); }
        }

        public string CurrentDuration
        {
            get => _currentDuration;
            set { _currentDuration = value; OnPropertyChanged(); }
        }

        public int CurrentProgress
        {
            get => _currentProgress;
            set { _currentProgress = value; OnPropertyChanged(); }
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set { _isPlaying = value; OnPropertyChanged(); }
        }

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set { _connectionStatus = value; OnPropertyChanged(); }
        }

        public bool IsConnected
        {
            get => _isConnected;
            set { _isConnected = value; OnPropertyChanged(); }
        }

        public ICommand PlayPauseCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand AddToPlaylistCommand { get; }

        public MainViewModel()
        {
            _trafficWatchClient = new TrafficWatchPluginClient();
            
            // تنظیم رویدادهای اتصال
            _trafficWatchClient.OnConnected += (s, e) =>
            {
                ConnectionStatus = "✅ متصل به TrafficWatch";
                IsConnected = true;
            };

            _trafficWatchClient.OnDisconnected += (s, e) =>
            {
                ConnectionStatus = "❌ قطع اتصال از TrafficWatch";
                IsConnected = false;
            };

            PlayPauseCommand = new RelayCommand(PlayPause);
            NextCommand = new RelayCommand(NextTrack);
            PreviousCommand = new RelayCommand(PreviousTrack);
            AddToPlaylistCommand = new RelayCommand<string>(AddToPlaylist);

            // شروع اتصال به TrafficWatch
            _ = InitializeTrafficWatchAsync();
        }

        private async Task InitializeTrafficWatchAsync()
        {
            try
            {
                await _trafficWatchClient.ConnectAsync();
                
                if (_trafficWatchClient.IsConnected)
                {
                    _ = _trafficWatchClient.StartListeningAsync();
                    
                    // ارسال داده اولیه
                    await UpdateTrafficWatchAsync();
                }
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"⚠️ خطا: {ex.Message}";
            }
        }

        private void LoadTrack(MusicTrack track)
        {
            CurrentTitle = track.Title;
            CurrentArtist = track.Artist;
            CurrentAlbum = track.Album;
            CurrentDuration = track.Duration;
            CurrentProgress = 0;
            
            _ = UpdateTrafficWatchAsync();
        }

        private void PlayPause()
        {
            IsPlaying = !IsPlaying;
            _ = UpdateTrafficWatchAsync();
        }

        private void NextTrack()
        {
            if (Playlist.Count == 0) return;
            
            var currentIndex = Playlist.IndexOf(SelectedTrack ?? Playlist[0]);
            var nextIndex = (currentIndex + 1) % Playlist.Count;
            SelectedTrack = Playlist[nextIndex];
        }

        private void PreviousTrack()
        {
            if (Playlist.Count == 0) return;
            
            var currentIndex = Playlist.IndexOf(SelectedTrack ?? Playlist[0]);
            var prevIndex = currentIndex > 0 ? currentIndex - 1 : Playlist.Count - 1;
            SelectedTrack = Playlist[prevIndex];
        }

        private void AddToPlaylist(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;

            var track = new MusicTrack
            {
                Title = Path.GetFileNameWithoutExtension(filePath),
                Artist = "نامشخص",
                Album = "نامشخص",
                FilePath = filePath,
                Duration = "0:00"
            };

            Playlist.Add(track);
            
            if (SelectedTrack == null)
            {
                SelectedTrack = track;
            }
        }

        private async Task UpdateTrafficWatchAsync()
        {
            if (!_trafficWatchClient.IsConnected)
                return;

            try
            {
                await _trafficWatchClient.SendStreamDataAsync(
                    CurrentTitle,
                    CurrentArtist,
                    CurrentAlbum,
                    CurrentDuration,
                    CurrentProgress,
                    IsPlaying
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MusicPlayerH] خطا در ارسال به TrafficWatch: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _trafficWatchClient.Dispose();
        }
    }

    /// <summary>
    /// مدل یک ترک موسیقی
    /// </summary>
    public class MusicTrack
    {
        public string Title { get; set; } = "";
        public string Artist { get; set; } = "";
        public string Album { get; set; } = "";
        public string Duration { get; set; } = "";
        public string FilePath { get; set; } = "";
    }
}

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Timers = System.Timers;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using Newtonsoft.Json;
using MediaTrack = MusicPlayerH.Models.MediaTrack;

namespace MusicPlayerH.Services
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly LibVLC _libVLC;
        private readonly MediaPlayer _mediaPlayer;
        private Timers.Timer? _positionTimer;
        private Timers.Timer? _heartbeatTimer;
        private bool _isTrafficWatchConnected;
        private readonly LanguageService _languageService;

        [ObservableProperty]
        private ObservableCollection<MediaTrack> _playlist = new();

        [ObservableProperty]
        private MediaTrack? _currentTrack;

        [ObservableProperty]
        private bool _isPlaying;

        [ObservableProperty]
        private double _position;

        [ObservableProperty]
        private double _duration;

        [ObservableProperty]
        private string _currentTime = "0:00";

        [ObservableProperty]
        private string _totalTime = "0:00";

        [ObservableProperty]
        private string _connectionStatus = "Disconnected";

        [ObservableProperty]
        private bool _isMuted;

        [ObservableProperty]
        private double _volume = 50;

        [ObservableProperty]
        private string _nowPlayingText = "No track selected";

        [ObservableProperty]
        private bool _isLanguageMenuOpen;

        [ObservableProperty]
        private string _currentLanguageDisplay = "فارسی";

        // Expose MediaPlayer for VideoView
        public MediaPlayer MediaPlayer => _mediaPlayer;

        public MainViewModel()
        {
            // Initialize Language Service
            _languageService = LanguageService.Instance;
            _languageService.Initialize();
            _currentLanguageDisplay = _languageService.CurrentLanguageName;

            // Initialize LibVLC for audio and video support
            Core.Initialize();
            _libVLC = new LibVLC(enableDebugLogs: false);
            _mediaPlayer = new MediaPlayer(_libVLC);

            // Setup media player events
            _mediaPlayer.Playing += (s, e) => IsPlaying = true;
            _mediaPlayer.Paused += (s, e) => IsPlaying = false;
            _mediaPlayer.Stopped += (s, e) => IsPlaying = false;
            _mediaPlayer.EndReached += (s, e) => PlayNext();
            _mediaPlayer.LengthChanged += (s, e) =>
            {
                Duration = e.Length / 1000.0;
                TotalTime = FormatTime(Duration);
            };

            // Setup position timer
            _positionTimer = new Timer(500); // Update every 500ms
            _positionTimer.Elapsed += OnPositionTimerElapsed;
            _positionTimer.Start();

            // Setup heartbeat timer for TrafficWatch
            _heartbeatTimer = new Timer(30000); // 30 seconds
            _heartbeatTimer.Elapsed += async (s, e) => await SendHeartbeatAsync();
            _heartbeatTimer.Start();

            // Initialize TrafficWatch connection
            InitializeTrafficWatchAsync();
        }

        private async void InitializeTrafficWatchAsync()
        {
            var client = TrafficWatchPluginClient.Instance;
            client.ConnectionStateChanged += (connected) =>
            {
                IsTrafficWatchConnected = connected;
                ConnectionStatus = connected ? "Connected to TrafficWatch" : "TrafficWatch not running";
            };

            await client.InitializeAsync();

            if (IsTrafficWatchConnected && CurrentTrack != null)
            {
                await SendNowPlayingAsync();
            }
        }

        private void OnPositionTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_mediaPlayer.IsPlaying)
                {
                    Position = _mediaPlayer.Position * 100;
                    var currentTime = _mediaPlayer.Time / 1000.0;
                    CurrentTime = FormatTime(currentTime);

                    // Send stream data to TrafficWatch periodically
                    if (IsTrafficWatchConnected && IsPlaying)
                    {
                        SendNowPlayingAsync();
                    }
                }
            });
        }

        private string FormatTime(double seconds)
        {
            if (double.IsNaN(seconds) || seconds < 0) return "0:00";
            
            var minutes = (int)(seconds / 60);
            var secs = (int)(seconds % 60);
            return $"{minutes}:{secs:D2}";
        }

        [RelayCommand]
        private void AddFiles()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Media Files|*.mp3;*.wav;*.flac;*.aac;*.ogg;*.wma;*.mp4;*.avi;*.mkv;*.mov;*.wmv;*.flv;*.webm|All Files|*.*",
                Multiselect = true,
                Title = "Select Music or Video Files"
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (var file in dialog.FileNames)
                {
                    var track = new MediaTrack
                    {
                        FilePath = file,
                        FileName = Path.GetFileName(file),
                        Title = Path.GetFileNameWithoutExtension(file),
                        Artist = "Unknown Artist",
                        Album = "Unknown Album",
                        Duration = GetMediaDuration(file)
                    };
                    Playlist.Add(track);
                }

                // Auto-play first track if nothing is playing
                if (CurrentTrack == null && Playlist.Count > 0)
                {
                    PlayTrack(Playlist[0]);
                }
            }
        }

        [RelayCommand]
        private void AddFolder()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select a folder containing media files",
                UseDescriptionForTitle = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var files = Directory.GetFiles(dialog.SelectedPath, "*.*")
                    .Where(f => IsMediaFile(f))
                    .OrderBy(f => f);

                foreach (var file in files)
                {
                    var track = new MediaTrack
                    {
                        FilePath = file,
                        FileName = Path.GetFileName(file),
                        Title = Path.GetFileNameWithoutExtension(file),
                        Artist = "Unknown Artist",
                        Album = "Unknown Album",
                        Duration = GetMediaDuration(file)
                    };
                    Playlist.Add(track);
                }

                if (CurrentTrack == null && Playlist.Count > 0)
                {
                    PlayTrack(Playlist[0]);
                }
            }
        }

        private bool IsMediaFile(string path)
        {
            var extensions = new[] { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", 
                                    ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm" };
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return extensions.Contains(ext);
        }

        private string GetMediaDuration(string filePath)
        {
            try
            {
                using var media = new Media(_libVLC, filePath);
                // This is a simplified version - in production you'd parse metadata properly
                return "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        [RelayCommand]
        private void PlayTrack(MediaTrack? track)
        {
            if (track == null) return;

            CurrentTrack = track;
            NowPlayingText = $"{track.Title} - {track.Artist}";

            var media = new Media(_libVLC, track.FilePath);
            _mediaPlayer.Media = media;
            _mediaPlayer.Play();

            // Send to TrafficWatch
            if (IsTrafficWatchConnected)
            {
                SendNowPlayingAsync();
            }
        }

        [RelayCommand]
        private void PlayPause()
        {
            if (_mediaPlayer.IsPlaying)
                _mediaPlayer.Pause();
            else if (CurrentTrack != null)
                _mediaPlayer.Play();
            else if (Playlist.Count > 0)
                PlayTrack(Playlist[0]);
        }

        [RelayCommand]
        private void Stop()
        {
            _mediaPlayer.Stop();
            IsPlaying = false;
            Position = 0;
            CurrentTime = "0:00";
        }

        [RelayCommand]
        private void PlayNext()
        {
            if (CurrentTrack == null || Playlist.Count == 0) return;

            var currentIndex = Playlist.IndexOf(CurrentTrack);
            var nextIndex = (currentIndex + 1) % Playlist.Count;
            PlayTrack(Playlist[nextIndex]);
        }

        [RelayCommand]
        private void PlayPrevious()
        {
            if (CurrentTrack == null || Playlist.Count == 0) return;

            var currentIndex = Playlist.IndexOf(CurrentTrack);
            var prevIndex = currentIndex > 0 ? currentIndex - 1 : Playlist.Count - 1;
            PlayTrack(Playlist[prevIndex]);
        }

        [RelayCommand]
        private void RemoveSelectedTrack(MediaTrack? track)
        {
            if (track == null) return;

            var wasPlaying = CurrentTrack == track;
            Playlist.Remove(track);

            if (wasPlaying)
            {
                Stop();
                CurrentTrack = null;
                NowPlayingText = "No track selected";
            }
        }

        [RelayCommand]
        private void ClearPlaylist()
        {
            Stop();
            Playlist.Clear();
            CurrentTrack = null;
            NowPlayingText = "No track selected";
        }

        partial void OnVolumeChanged(double value)
        {
            _mediaPlayer.Volume = (int)value;
        }

        partial void OnPositionChanged(double value)
        {
            if (!_mediaPlayer.IsPlaying) return;
            _mediaPlayer.Position = (float)(value / 100.0);
        }

        partial void OnCurrentLanguageDisplayChanged(string value)
        {
            // Update UI texts when language changes
            UpdateUiTexts();
        }

        private void UpdateUiTexts()
        {
            // Update connection status based on current language
            ConnectionStatus = IsTrafficWatchConnected 
                ? GetLocalizedString("ConnectedStatus") 
                : GetLocalizedString("DisconnectedStatus");
            
            NowPlayingText = CurrentTrack != null 
                ? $"{CurrentTrack.Title} - {CurrentTrack.Artist}" 
                : GetLocalizedString("NoTrackSelected");
        }

        private string GetLocalizedString(string key)
        {
            try
            {
                if (Application.Current.Resources[key] is string value)
                {
                    return value;
                }
            }
            catch { }
            return key;
        }

        [RelayCommand]
        private void SetLanguageFA()
        {
            _languageService.LoadLanguage(SupportedLanguage.FA);
            CurrentLanguageDisplay = _languageService.CurrentLanguageName;
            IsLanguageMenuOpen = false;
        }

        [RelayCommand]
        private void SetLanguageAR()
        {
            _languageService.LoadLanguage(SupportedLanguage.AR);
            CurrentLanguageDisplay = _languageService.CurrentLanguageName;
            IsLanguageMenuOpen = false;
        }

        [RelayCommand]
        private void SetLanguageEN()
        {
            _languageService.LoadLanguage(SupportedLanguage.EN);
            CurrentLanguageDisplay = _languageService.CurrentLanguageName;
            IsLanguageMenuOpen = false;
        }

        [RelayCommand]
        private void ToggleLanguageMenu()
        {
            IsLanguageMenuOpen = !IsLanguageMenuOpen;
        }

        private async Task SendNowPlayingAsync()
        {
            if (CurrentTrack == null || !IsTrafficWatchConnected) return;

            var payload = new
            {
                type = "now_playing",
                title = CurrentTrack.Title,
                artist = CurrentTrack.Artist,
                album = CurrentTrack.Album,
                duration = CurrentTrack.Duration,
                progress = (int)Position,
                isPlaying = IsPlaying
            };

            var message = new
            {
                action = "stream_data",
                timestamp = DateTime.UtcNow.ToString("o"),
                payload
            };

            await TrafficWatchPluginClient.Instance.SendDataAsync(JsonConvert.SerializeObject(message));
        }

        private async Task SendHeartbeatAsync()
        {
            if (!IsTrafficWatchConnected) return;

            var message = new { action = "heartbeat" };
            await TrafficWatchPluginClient.Instance.SendDataAsync(JsonConvert.SerializeObject(message));
        }

        public void Dispose()
        {
            _positionTimer?.Stop();
            _positionTimer?.Dispose();
            _heartbeatTimer?.Stop();
            _heartbeatTimer?.Dispose();
            _mediaPlayer.Dispose();
            _libVLC.Dispose();
        }
    }
}

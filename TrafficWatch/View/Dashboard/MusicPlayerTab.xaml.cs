using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using TrafficWatch.Models.Dashboard;
using TrafficWatch.Services.Dashboard;

namespace TrafficWatch.View.Dashboard
{
    /// <summary>
    /// Interaction logic for MusicPlayerTab.xaml
    /// کاربرگ پخش کننده موسیقی و ویدئو
    /// </summary>
    public partial class MusicPlayerTab : UserControl
    {
        private readonly MusicPlayerService _musicService;
        private bool _isPlaying;
        
        // منابع چندزبانه
        private static readonly string[] Languages = { "fa", "en", "ar" };
        private int _currentLanguageIndex;
        
        public MusicPlayerTab()
        {
            InitializeComponent();
            
            _musicService = new MusicPlayerService();
            _musicService.Initialize();
            
            // اتصال به رویدادها
            _musicService.OnPlaybackStateChanged += MusicService_OnPlaybackStateChanged;
            _musicService.OnTrackChanged += MusicService_OnTrackChanged;
            _musicService.OnPlaylistUpdated += MusicService_OnPlaylistUpdated;
            
            _isPlaying = false;
            _currentLanguageIndex = 0; // پیش‌فرض فارسی
            
            LoadLanguageSettings();
        }
        
        /// <summary>
        /// بارگذاری تنظیمات زبان
        /// </summary>
        private void LoadLanguageSettings()
        {
            var addon = DashboardAddonService.Instance.GetAddonById("music-player");
            if (addon?.Settings != null && addon.Settings.ContainsKey("Language"))
            {
                string lang = addon.Settings["Language"].ToString();
                _currentLanguageIndex = Array.IndexOf(Languages, lang);
                if (_currentLanguageIndex < 0) _currentLanguageIndex = 0;
            }
            
            ApplyLanguage();
        }
        
        /// <summary>
        /// اعمال زبان به UI
        /// </summary>
        private void ApplyLanguage()
        {
            string lang = Languages[_currentLanguageIndex];
            
            // بروزرسانی متن‌ها بر اساس زبان
            switch (lang)
            {
                case "fa":
                    // فارسی
                    break;
                case "en":
                    // انگلیسی
                    break;
                case "ar":
                    // عربی
                    FlowDirection = FlowDirection.RightToLeft;
                    break;
            }
            
            // ذخیره تنظیمات
            var addon = DashboardAddonService.Instance.GetAddonById("music-player");
            if (addon?.Settings != null)
            {
                addon.Settings["Language"] = lang;
                DashboardAddonService.Instance.UpdateAddonSettings("music-player", addon.Settings);
            }
        }
        
        /// <summary>
        /// مدیریت تغییر وضعیت پخش
        /// </summary>
        private void MusicService_OnPlaybackStateChanged(object sender, PlaybackStateChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _isPlaying = e.State == PlaybackState.Playing;
                btnPlayPause.Content = _isPlaying ? "⏸" : "▶";
                
                if (e.Track != null)
                {
                    progressPlayback.Value = e.Track.CurrentPosition;
                    lblCurrentTime.Text = FormatTime(e.Track.CurrentPosition);
                    lblTotalTime.Text = FormatTime(e.Track.Duration);
                }
            });
        }
        
        /// <summary>
        /// مدیریت تغییر ترک
        /// </summary>
        private void MusicService_OnTrackChanged(object sender, TrackChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.Track != null)
                {
                    lblTrackTitle.Text = e.Track.Title;
                    lblTrackArtist.Text = e.Track.Artist ?? "Unknown Artist";
                    lblTrackAlbum.Text = e.Track.Album ?? "Unknown Album";
                    
                    // نمایش کاور آلبوم
                    if (e.Track.CoverArt != null)
                    {
                        // TODO: تبدیل بایت‌آرایه به تصویر
                    }
                    
                    lblTotalTime.Text = FormatTime(e.Track.Duration);
                }
                else
                {
                    lblTrackTitle.Text = "No Track Playing";
                    lblTrackArtist.Text = "";
                    lblTrackAlbum.Text = "";
                    lblTotalTime.Text = "0:00";
                }
            });
        }
        
        /// <summary>
        /// مدیریت بروزرسانی لیست پخش
        /// </summary>
        private void MusicService_OnPlaylistUpdated(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var playlist = _musicService.GetPlaylist();
                lstPlaylist.ItemsSource = playlist;
                lblTrackCount.Text = playlist.Count.ToString();
            });
        }
        
        /// <summary>
        /// فرمت کردن زمان به صورت mm:ss
        /// </summary>
        private string FormatTime(int seconds)
        {
            int mins = seconds / 60;
            int secs = seconds % 60;
            return $"{mins}:{secs:D2}";
        }
        
        /// <summary>
        /// دکمه پخش/توقف
        /// </summary>
        private async void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                await _musicService.PauseAsync();
            }
            else
            {
                var currentTrack = _musicService.GetCurrentTrack();
                if (currentTrack != null)
                {
                    await _musicService.ResumeAsync();
                }
                else if (lstPlaylist.SelectedItem is MediaTrackInfo track)
                {
                    await _musicService.PlayTrackAsync(track);
                }
                else if (_musicService.GetPlaylist().Count > 0)
                {
                    await _musicService.PlayTrackAsync(_musicService.GetPlaylist()[0]);
                }
            }
        }
        
        /// <summary>
        /// دکمه توقف
        /// </summary>
        private async void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            await _musicService.StopAsync();
        }
        
        /// <summary>
        /// دکمه ترک بعدی
        /// </summary>
        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            _musicService.PlayNext();
        }
        
        /// <summary>
        /// دکمه ترک قبلی
        /// </summary>
        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            _musicService.PlayPrevious();
        }
        
        /// <summary>
        /// دکمه تغییر حالت تصادفی
        /// </summary>
        private void BtnShuffle_Click(object sender, RoutedEventArgs e)
        {
            _musicService.ToggleShuffle();
            btnShuffle.Opacity = _musicService.GetPlaylist().Count > 0 ? 1.0 : 0.5;
        }
        
        /// <summary>
        /// دکمه تغییر حالت تکرار
        /// </summary>
        private void BtnRepeat_Click(object sender, RoutedEventArgs e)
        {
            _musicService.CycleRepeatMode();
        }
        
        /// <summary>
        /// دکمه استریم
        /// </summary>
        private async void BtnStream_Click(object sender, RoutedEventArgs e)
        {
            var menu = new ContextMenu();
            
            var streamToNetwork = new MenuItem { Header = "Stream to Network" };
            streamToNetwork.Click += async (s, args) =>
            {
                // TODO: نمایش دیالوگ برای وارد کردن IP و پورت
                await _musicService.StartNetworkStreamingAsync("192.168.1.100", 8080);
            };
            
            var streamToTV = new MenuItem { Header = "Stream to TV (DLNA)" };
            streamToTV.Click += async (s, args) =>
            {
                // TODO: جستجوی دستگاه‌های DLNA در شبکه
                await _musicService.StartTVStreamingAsync("192.168.1.200");
            };
            
            menu.Items.Add(streamToNetwork);
            menu.Items.Add(streamToTV);
            
            menu.IsOpen = true;
        }
        
        /// <summary>
        /// دکمه افزودن فایل
        /// </summary>
        private async void BtnAddFiles_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Media Files|*.mp3;*.wav;*.flac;*.aac;*.ogg;*.m4a;*.mp4;*.mkv;*.avi;*.mov;*.wmv|Audio Files|*.mp3;*.wav;*.flac;*.aac;*.ogg;*.m4a|Video Files|*.mp4;*.mkv;*.avi;*.mov;*.wmv|All Files|*.*"
            };
            
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    await _musicService.AddToPlaylistAsync(file);
                }
            }
        }
        
        /// <summary>
        /// دکمه افزودن پوشه
        /// </summary>
        private async void BtnAddFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select folder containing media files",
                ShowNewFolderButton = false
            };
            
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] extensions = { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".m4a", ".mp4", ".mkv", ".avi", ".mov", ".wmv" };
                var files = Directory.GetFiles(folderDialog.SelectedPath)
                    .Where(f => extensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                    .ToArray();
                
                foreach (string file in files)
                {
                    await _musicService.AddToPlaylistAsync(file);
                }
            }
        }
        
        /// <summary>
        /// دکمه پاک کردن لیست پخش
        /// </summary>
        private void BtnClearPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the playlist?", 
                "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _musicService.ClearPlaylist();
            }
        }
        
        /// <summary>
        /// انتخاب ترک از لیست
        /// </summary>
        private async void LstPlaylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstPlaylist.SelectedItem is MediaTrackInfo track)
            {
                await _musicService.PlayTrackAsync(track);
            }
        }
        
        /// <summary>
        /// تغییر حجم صدا
        /// </summary>
        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_musicService != null)
            {
                _musicService.SetVolume((int)sliderVolume.Value);
                lblVolume.Text = $"{(int)sliderVolume.Value}%";
            }
        }
        
        /// <summary>
        /// تمیزکاری هنگام بسته شدن
        /// </summary>
        public void Cleanup()
        {
            _musicService.Stop();
        }
    }
}

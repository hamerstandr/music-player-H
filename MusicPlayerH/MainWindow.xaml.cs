using System.Windows;
using LibVLCSharp.WPF;
using MusicPlayerH.Services;

namespace MusicPlayerH
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private VideoView? _videoView;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            // Initialize VideoView for video playback
            InitializeVideoView();

            Closed += (s, e) => _viewModel.Dispose();
        }

        private void InitializeVideoView()
        {
            _videoView = new VideoView
            {
                MediaPlayer = _viewModel.MediaPlayer
            };
            
            VideoView.Content = _videoView;
        }
    }
}

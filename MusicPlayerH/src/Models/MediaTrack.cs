namespace MusicPlayerH.Models
{
    public class MediaTrack
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = "Unknown Artist";
        public string Album { get; set; } = "Unknown Album";
        public string Duration { get; set; } = "Unknown";
        public string? CoverArtPath { get; set; }
    }
}

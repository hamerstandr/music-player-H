using System;

namespace MusicPlayerH.Models.Dashboard
{
    /// <summary>
    /// مدل اطلاعات ترک موسیقی/ویدئو
    /// </summary>
    public class MediaTrackInfo
    {
        /// <summary>
        /// شناسه یکتا
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// عنوان ترک
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// نام هنرمند
        /// </summary>
        public string Artist { get; set; }
        
        /// <summary>
        /// نام آلبوم
        /// </summary>
        public string Album { get; set; }
        
        /// <summary>
        /// مدت زمان (ثانیه)
        /// </summary>
        public int Duration { get; set; }
        
        /// <summary>
        /// مسیر فایل
        /// </summary>
        public string FilePath { get; set; }
        
        /// <summary>
        /// نوع فایل (Audio/Video)
        /// </summary>
        public string MediaType { get; set; } = "Audio";
        
        /// <summary>
        /// فرمت فایل (mp3, flac, mp4, mkv, etc.)
        /// </summary>
        public string Format { get; set; }
        
        /// <summary>
        /// کیفیت فایل
        /// </summary>
        public string Quality { get; set; }
        
        /// <summary>
        /// کدک صوتی
        /// </summary>
        public string AudioCodec { get; set; }
        
        /// <summary>
        /// کدک ویدئویی (برای فایل‌های ویدئویی)
        /// </summary>
        public string VideoCodec { get; set; }
        
        /// <summary>
        /// بیت‌ریت
        /// </summary>
        public int Bitrate { get; set; }
        
        /// <summary>
        /// نرخ نمونه‌برداری
        /// </summary>
        public int SampleRate { get; set; }
        
        /// <summary>
        /// تعداد کانال‌ها
        /// </summary>
        public int Channels { get; set; }
        
        /// <summary>
        /// وضعیت پخش (در حال پخش، متوقف، etc.)
        /// </summary>
        public PlaybackState State { get; set; }
        
        /// <summary>
        /// موقعیت فعلی پخش (ثانیه)
        /// </summary>
        public int CurrentPosition { get; set; }
        
        /// <summary>
        /// تصویر کاور آلبوم
        /// </summary>
        public byte[] CoverArt { get; set; }
        
        /// <summary>
        /// تاریخ اضافه شدن به لیست پخش
        /// </summary>
        public DateTime DateAdded { get; set; } = DateTime.Now;
        
        /// <summary>
        /// تعداد دفعات پخش
        /// </summary>
        public int PlayCount { get; set; }
        
        /// <summary>
        /// امتیاز کاربر (1-5)
        /// </summary>
        public int Rating { get; set; }
        
        /// <summary>
        /// آیا این فایل از شبکه استریم می‌شود؟
        /// </summary>
        public bool IsStreaming { get; set; }
        
        /// <summary>
        /// آدرس استریم (برای محتوای آنلاین)
        /// </summary>
        public string StreamUrl { get; set; }
    }
    
    /// <summary>
    /// وضعیت پخش
    /// </summary>
    public enum PlaybackState
    {
        Stopped,
        Playing,
        Paused,
        Buffering,
        Error
    }
}

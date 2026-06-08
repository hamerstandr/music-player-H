using System.Collections.Generic;
using TrafficWatch.Models.Dashboard;

namespace TrafficWatch.Models.Dashboard
{
    /// <summary>
    /// مدل اطلاعات افزونه پخش کننده موسیقی
    /// </summary>
    public class MusicPlayerAddonInfo : AddonInfo
    {
        public MusicPlayerAddonInfo()
        {
            Id = "music-player";
            Name = "Music Player";
            Description = "Professional music and video player with streaming capabilities";
            Version = "1.0.0";
            Author = "TrafficWatch Team";
            ApiPort = 9091; // پورت اختصاصی موزیک پلیر
            DisplayOrder = 2; // نمایش بعد از دانلود منیجر
            
            // تنظیمات پیش‌فرض
            Settings = new Dictionary<string, object>
            {
                { "Language", "fa" }, // fa, en, ar
                { "Volume", 75 },
                { "Shuffle", false },
                { "Repeat", "none" }, // none, one, all
                { "ShowVideoAsAudio", true }, // پردازش فایل‌های ویدئویی به عنوان صوتی
                { "EnableNetworkStreaming", true },
                { "EnableDLNA", true },
                { "CodecPriority", "auto" }
            };
        }
    }
}

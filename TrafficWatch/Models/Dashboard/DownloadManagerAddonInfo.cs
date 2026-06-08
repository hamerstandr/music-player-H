using System.Collections.Generic;
using TrafficWatch.Models.Dashboard;

namespace TrafficWatch.Models.Dashboard
{
    /// <summary>
    /// مدل اطلاعات افزونه دانلود منیجر
    /// </summary>
    public class DownloadManagerAddonInfo : AddonInfo
    {
        public DownloadManagerAddonInfo()
        {
            Id = "download-manager";
            Name = "Download Manager";
            Description = "Monitor and manage downloads with DownloadMenger2";
            Version = "1.0.0";
            Author = "TrafficWatch Team";
            ApiPort = 9090; // پورت پیش‌فرض DownloadMenger2
            DisplayOrder = 1; // اولویت اول
            
            Settings = new Dictionary<string, object>
            {
                { "ShowSpeed", true },
                { "ShowProgress", true },
                { "MaxActiveDownloads", 5 },
                { "AutoStart", false }
            };
        }
    }
}

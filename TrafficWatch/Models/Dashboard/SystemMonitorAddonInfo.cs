using System.Collections.Generic;
using TrafficWatch.Models.Dashboard;

namespace TrafficWatch.Models.Dashboard
{
    /// <summary>
    /// مدل اطلاعات افزونه مانیتور سیستم
    /// </summary>
    public class SystemMonitorAddonInfo : AddonInfo
    {
        public SystemMonitorAddonInfo()
        {
            Id = "system-monitor";
            Name = "System Monitor";
            Description = "Monitor CPU, RAM, Network and Disk usage";
            Version = "1.0.0";
            Author = "TrafficWatch Team";
            ApiPort = 0; // بدون API خارجی
            DisplayOrder = 3;
            
            Settings = new Dictionary<string, object>
            {
                { "ShowCPU", true },
                { "ShowRAM", true },
                { "ShowNetwork", true },
                { "ShowDisk", true },
                { "RefreshInterval", 2 },
                { "ShowGraphs", true }
            };
        }
    }
}

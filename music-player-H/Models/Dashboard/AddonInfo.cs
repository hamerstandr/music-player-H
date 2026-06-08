using System;
using System.Collections.Generic;

namespace MusicPlayerH.Models.Dashboard
{
    /// <summary>
    /// کلاس پایه برای اطلاعات افزونه‌های داشبورد
    /// </summary>
    public abstract class AddonInfo
    {
        /// <summary>
        /// شناسه یکتای افزونه
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// نام نمایشی افزونه
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// توضیحات افزونه
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// نسخه افزونه
        /// </summary>
        public string Version { get; set; }
        
        /// <summary>
        /// نویسنده افزونه
        /// </summary>
        public string Author { get; set; }
        
        /// <summary>
        /// پورت API اختصاصی افزونه
        /// </summary>
        public int ApiPort { get; set; }
        
        /// <summary>
        /// ترتیب نمایش در داشبورد
        /// </summary>
        public int DisplayOrder { get; set; }
        
        /// <summary>
        /// آیا افزونه نصب شده است؟
        /// </summary>
        public bool IsInstalled { get; set; } = false;
        
        /// <summary>
        /// آیا افزونه فعال است؟
        /// </summary>
        public bool IsEnabled { get; set; } = true;
        
        /// <summary>
        /// آیا افزونه در حال اجرا است؟
        /// </summary>
        public bool IsRunning { get; set; } = false;
        
        /// <summary>
        /// تنظیمات اختصاصی افزونه
        /// </summary>
        public Dictionary<string, object> Settings { get; set; } = new Dictionary<string, object>();
        
        /// <summary>
        /// تاریخ آخرین بروزرسانی
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        
        /// <summary>
        /// مسیر executable برنامه خارجی (در صورت وجود)
        /// </summary>
        public string ExecutablePath { get; set; }
        
        /// <summary>
        /// آیکون افزونه (اختیاری)
        /// </summary>
        public string IconPath { get; set; }
    }
}

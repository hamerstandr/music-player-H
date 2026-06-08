# 🎵 TrafficWatch v2.0 - داشبورد هوشمند مانیتورینگ

[![Version](https://img.shields.io/badge/version-2.0.0-blue.svg)](https://github.com/hamerstandr/TrafficWatch/releases)
[![.NET](https://img.shields.io/badge/.NET-6.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-lightgrey.svg)]()
[![License](https://img.shields.io/badge/license-MIT-green.svg)]()

## 📖 درباره پروژه

TrafficWatch یک داشبورد مدرن و قابل گسترش برای مانیتورینگ سیستم است که با استفاده از **WPF** و **C#** توسعه یافته است. این برنامه دارای یک سیستم افزونه‌ای پیشرفته است که امکان اضافه کردن ماژول‌های مختلف را فراهم می‌کند.

### ✨ ویژگی‌های نسخه 2.0

#### 🎵 Music Player Addon (جدید!)
- پخش حرفه‌ای فایل‌های صوتی و ویدئویی
- لیست پخش پیشرفته با Drag & Drop
- پشتیبانی از فرمت‌های متعدد:
  - **صوتی:** MP3, WAV, FLAC, AAC, OGG, M4A, WMA, ALAC
  - **ویدئویی:** MP4, MKV, AVI, MOV, WMV, FLV, WebM
- حالت‌های پخش: Shuffle, Repeat One, Repeat All
- استریم به شبکه محلی و تلویزیون (DLNA/Chromecast)
- چندزبانه: فارسی، انگلیسی، عربی
- مدیریت کدک‌های صوتی و تصویری

#### ⬇️ Download Manager Integration
- اتصال به DownloadMenger2 از طریق API
- نمایش وضعیت دانلودها
- کنترل دانلودها از داشبورد

#### 📊 System Monitor
- نمایش مصرف CPU، RAM، Disk
- نمودارهای زنده
- تاریخچه مصرف

#### 🔌 سیستم افزونه‌ای
- نصب آسان افزونه‌ها
- فعال/غیرفعال کردن هر افزونه
- تغییر ترتیب نمایش
- API برای ارتباط با برنامه‌های خارجی

---

## 🚀 شروع سریع

### پیش‌نیازها

- **ویندوز:** Windows 10/11 (x64)
- **.NET:** .NET 6 Desktop Runtime ([دانلود](https://dotnet.microsoft.com/download/dotnet/6.0))
- **Visual Studio:** نسخه 2022 یا بالاتر (برای توسعه)

### نصب و اجرا

#### روش 1: دانلود از Releases
1. به صفحه [Releases](https://github.com/hamerstandr/TrafficWatch/releases) بروید
2. آخرین نسخه را دانلود کنید
3. از حالت فشرده خارج کنید
4. فایل `TrafficWatch.exe` را اجرا کنید

#### روش 2: بیلد از سورس
```bash
git clone https://github.com/hamerstandr/TrafficWatch.git
cd TrafficWatch
dotnet restore
dotnet run
```

#### روش 3: پابلیش حرفه‌ای
```bash
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -o ./publish
```

---

## 📁 ساختار پروژه

```
TrafficWatch/
├── Models/Dashboard/          # مدل‌های داده
│   ├── AddonInfo.cs           # کلاس پایه افزونه‌ها
│   ├── MediaTrackInfo.cs      # اطلاعات ترک موسیقی
│   ├── MusicPlayerAddonInfo.cs
│   ├── DownloadManagerAddonInfo.cs
│   └── SystemMonitorAddonInfo.cs
│
├── Services/Dashboard/        # سرویس‌ها
│   ├── DashboardAddonService.cs    # مدیریت افزونه‌ها
│   └── MusicPlayerService.cs       # سرویس موزیک پلیر
│
├── View/Dashboard/            # رابط کاربری
│   └── MusicPlayerTab.xaml    # تب موزیک پلیر
│
├── ViewModel/Dashboard/       # ViewModelها
│
├── Properties/                # تنظیمات پروژه
│   ├── AssemblyInfo.cs
│   └── Settings.settings
│
├── Resources/                 # منابع (آیکون، تصاویر)
│
├── TrafficWatch.csproj        # فایل پروژه
├── App.xaml                   # نقطه شروع برنامه
└── MainWindow.xaml            # پنجره اصلی
```

---

## 🛠️ توسعه

### افزودن افزونه جدید

1. **ایجاد مدل:**
```csharp
public class MyAddonInfo : AddonInfo
{
    public MyAddonInfo()
    {
        Id = "my-addon";
        Name = "My Addon";
        ApiPort = 9092;
    }
}
```

2. **ایجاد سرویس:**
```csharp
public class MyAddonService
{
    // منطق افزونه
}
```

3. **ایجاد UI:**
```xml
<UserControl x:Class="...MyAddonTab">
    <!-- UI elements -->
</UserControl>
```

4. **ثبت افزونه:**
در `DashboardAddonService.RegisterDefaultAddons()`:
```csharp
_addons.Add(new MyAddonInfo());
```

### مستندات بیشتر

- 📚 [مستندات کامل افزونه‌ها](docs/MusicPlayerAddonDocumentation.md)
- 📦 [راهنمای پابلیش](PUBLISH_GUIDE.md)
- ⚡ [راهنمای سریع بیلد](QUICK_BUILD.md)

---

## 🎯 API Endpoints

| افزونه | پورت | Endpoint |
|--------|------|----------|
| Music Player | 9091 | `/api/status`, `/api/play`, `/api/playlist` |
| Download Manager | 9090 | `/api/status`, `/api/downloads` |
| System Monitor | - | Internal |

---

## 🌐 چندزبانه

برنامه از 3 زبان پشتیبانی می‌کند:

| زبان | کد | RTL |
|------|-----|-----|
| فارسی | fa-IR | ✅ |
| عربی | ar-SA | ✅ |
| انگلیسی | en-US | ❌ |

تغییر زبان از طریق تنظیمات موزیک پلیر امکان‌پذیر است.

---

## 🧪 تست

### اجرای تست‌ها
```bash
dotnet test
```

### چک‌لیست کیفیت
- ✅ بیلد بدون خطا
- ✅ تمام افزونه‌ها لود می‌شوند
- ✅ UI روان است
- ✅ مصرف RAM منطقی (<200MB)
- ✅ بدون Memory Leak

---

## 📊 مقایسه نسخه‌ها

| ویژگی | v1.0 | v2.0 |
|-------|------|------|
| تعداد افزونه‌ها | 2 | 3 |
| موزیک پلیر | ❌ | ✅ |
| چندزبانه | ❌ | ✅ |
| استریمینگ | ❌ | ✅ |
| سیستم افزونه‌ای | ساده | پیشرفته |

---

## 🤝 مشارکت

از مشارکت شما استقبال می‌کنیم!

### نحوه مشارکت
1. Fork کنید
2. Branch بسازید (`git checkout -b feature/NewFeature`)
3. Commit کنید (`git commit -m 'Add new feature'`)
4. Push کنید (`git push origin feature/NewFeature`)
5. Pull Request باز کنید

### گزارش باگ
از [Issues](https://github.com/hamerstandr/TrafficWatch/issues) استفاده کنید.

---

## 📄 مجوز

این پروژه تحت مجوز **MIT** منتشر شده است.

```
Copyright (c) 2024 TrafficWatch Team

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software.
```

---

## 👥 تیم توسعه

- **توسعه‌دهنده اصلی:** TrafficWatch Team
- **طراح UI/UX:** TrafficWatch Design Team
- **مستندات:** Technical Writing Team

---

## 📞 تماس

- 📧 Email: support@trafficwatch.ir
- 💬 GitHub Issues: [اینجا](https://github.com/hamerstandr/TrafficWatch/issues)
- 🌐 وبسایت: https://trafficwatch.ir

---

## 🙏 تشکر و قدردانی

از کتابخانه‌های زیر استفاده شده است:

- [NAudio](https://github.com/NAudio/NAudio) - پردازش صدا
- [TagLibSharp](https://github.com/taglib/taglib-sharp) - متادیتای فایل‌های مدیا
- [Newtonsoft.Json](https://www.newtonsoft.com/json) - پردازش JSON
- [System.Management](https://www.nuget.org/packages/System.Management) - دسترسی به اطلاعات سیستم

---

## 📈 آمار پروژه

- **تعداد فایل‌ها:** 15+
- **تعداد خطوط کد:** ~1800
- **تعداد افزونه‌ها:** 3
- **حجم نهایی:** ~15MB (Single File)
- **زمان لود:** <3 ثانیه

---

<div align="center">

**ساخته شده با ❤️ توسط TrafficWatch Team**

⭐ اگر خوشتان آمد، ستاره دهید!

</div>

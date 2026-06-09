# 📖 راهنمای توسعه Music Player H

<div dir="rtl">

## فهرست مطالب

1. [معرفی سیستم](#معرفی-سیستم)
2. [معماری سیستم](#معماری-سیستم)
3. [راه‌اندازی محیط توسعه](#راه‌اندازی-محیط-توسعه)
4. [ایجاد قابلیت‌های جدید](#ایجاد-قابلیت‌های-جدید)
5. [یکپارچه‌سازی با TrafficWatch](#یکپارچه‌سازی-با-trafficwatch)
6. [استریمینگ](#استریمینگ)
7. [تنظیمات و پیکربندی](#تنظیمات-و-پیکربندی)
8. [نمونه کدها](#نمونه-کدها)

---

## معرفی سیستم

**Music Player H** یک پخش‌کننده موسیقی و ویدئوی حرفه‌ای است که به عنوان یک افزونه برای داشبورد TrafficWatch طراحی شده است. این برنامه قابلیت‌های زیر را ارائه می‌دهد:

### ویژگی‌های کلیدی

1. **پخش حرفه‌ای**: پخش/توقف/مکث/ادامه با کنترل کامل
2. **لیست پخش پیشرفته**: مدیریت هوشمند لیست‌های پخش
3. **پشتیبانی از فرمت‌های متنوع**: صوتی و ویدئویی
4. **استریمینگ شبکه**: استریم به شبکه، تلویزیون و TrafficWatch
5. **چندزبانه**: فارسی، انگلیسی، عربی
6. **کدک‌های پیشرفته**: پشتیبانی از کدک‌های مختلف صوتی و ویدئویی

---

## معماری سیستم

```
┌─────────────────────────────────────────────────────────┐
│                   Music Player H                        │
├─────────────────────────────────────────────────────────┤
│  ┌─────────────────────────────────────────────────┐   │
│  │              View Layer (WPF)                    │   │
│  │  ┌─────────────────────────────────────────┐    │   │
│  │  │         MusicPlayerTab.xaml             │    │   │
│  │  │  - UI Controls                           │    │   │
│  │  │  - Event Handlers                        │    │   │
│  │  └─────────────────────────────────────────┘    │   │
│  └─────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────┤
│  ┌─────────────────────────────────────────────────┐   │
│  │              Service Layer                       │   │
│  │  ┌─────────────────────────────────────────┐    │   │
│  │  │       MusicPlayerService.cs             │    │   │
│  │  │  - Playback Control                      │    │   │
│  │  │  - Playlist Management                   │    │   │
│  │  │  - Streaming                             │    │   │
│  │  └─────────────────────────────────────────┘    │   │
│  └─────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────┤
│  ┌─────────────────────────────────────────────────┐   │
│  │              Model Layer                         │   │
│  │  - MediaTrackInfo                               │   │
│  │  - MusicPlayerAddonInfo                         │   │
│  └─────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────┤
│  ┌─────────────────────────────────────────────────┐   │
│  │          External Integrations                   │   │
│  │  - TrafficWatch Dashboard (Port 9091)           │   │
│  │  - Network Streaming (HTTP)                     │   │
│  │  - DLNA/Chromecast                              │   │
│  └─────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

### ساختار فایل‌ها

```
music-player-H/
├── Models/
│   └── Dashboard/
│       ├── AddonInfo.cs              # کلاس پایه افزونه
│       ├── MediaTrackInfo.cs         # مدل اطلاعات ترک
│       └── MusicPlayerAddonInfo.cs   # مدل افزونه موزیک
│
├── Services/
│   └── Dashboard/
│       ├── DashboardAddonService.cs  # مدیریت افزونه‌ها
│       └── MusicPlayerService.cs     # سرویس پخش موسیقی
│
├── View/
│   └── Dashboard/
│       ├── MusicPlayerTab.xaml       # رابط کاربری
│       └── MusicPlayerTab.xaml.cs    # کد پشت صحنه
│
├── Properties/
│   └── AssemblyInfo.cs               # اطلاعات اسمبلی
│
├── Resources/
│   ├── app.ico                       # آیکون برنامه
│   └── Images/                       # تصاویر
│
└── music-player-H.csproj             # فایل پروژه
```

---

## راه‌اندازی محیط توسعه

### پیش‌نیازها

1. **Visual Studio 2022** (Community یا بالاتر)
   - Workload: .NET Desktop Development
   - Component: .NET 6.0 Runtime

2. **.NET 6.0 SDK**
   ```bash
   dotnet --version
   ```

3. **NuGet Packages** (به صورت خودکار نصب می‌شوند):
   - NAudio (پخش صوتی)
   - TagLibSharp (متادیتا)
   - Newtonsoft.Json (JSON)

### مراحل راه‌اندازی

1. **کلون کردن پروژه**:
   ```bash
   git clone <repository-url>
   cd music-player-H
   ```

2. **باز کردن در Visual Studio**:
   - File > Open > Project/Solution
   - انتخاب `music-player-H.csproj`

3. **Restore Dependencies**:
   ```bash
   dotnet restore
   ```

4. **بیلد اولیه**:
   ```bash
   dotnet build -c Debug
   ```

---

## ایجاد قابلیت‌های جدید

### افزودن فرمت جدید

برای پشتیبانی از فرمت جدید، متد `IsVideoFormat` را در `MusicPlayerService.cs` ویرایش کنید:

```csharp
private bool IsVideoFormat(string extension)
{
    var videoFormats = new[] { 
        ".mp4", ".mkv", ".avi", ".mov", ".wmv", ".flv", ".webm",
        // فرمت جدید را اضافه کنید
        ".newformat"
    };
    return videoFormats.Contains(extension.ToLowerInvariant());
}
```

### افزودن زبان جدید

1. در `MusicPlayerTab.xaml.cs`، آرایه `Languages` را ویرایش کنید:
   ```csharp
   private static readonly string[] Languages = { "fa", "en", "ar", "de" };
   ```

2. در متد `ApplyLanguage`، کیس جدید اضافه کنید:
   ```csharp
   case "de":
       // آلمانی
       break;
   ```

---

## یکپارچه‌سازی با TrafficWatch

### ثبت افزونه در TrafficWatch

در فایل `DashboardAddonService.cs`، افزونه موزیک پلیر به صورت پیش‌فرض ثبت شده است:

```csharp
private void RegisterDefaultAddons()
{
    if (!_addons.Any(a => a.Id == "music-player"))
    {
        _addons.Add(new MusicPlayerAddonInfo());
    }
}
```

### ارتباط API

موزیک پلیر از پورت **9091** برای ارتباط با TrafficWatch استفاده می‌کند:

```csharp
var addon = DashboardAddonService.Instance.GetAddonById("music-player");
string apiEndpoint = DashboardAddonService.Instance.GetAddonApiEndpoint("music-player");
// خروجی: http://127.0.0.1:9091
```

### دریافت وضعیت از TrafficWatch

```csharp
var status = await MusicPlayerService.GetStatusAsync();
// ارسال به TrafficWatch
await TrafficWatchApi.SendUpdateAsync(status);
```

---

## استریمینگ

### استریم به شبکه محلی

```csharp
public async Task<bool> StartNetworkStreamingAsync(string ipAddress, int port)
{
    // ایجاد سرور HTTP داخلی
    var server = new HttpListener();
    server.Prefixes.Add($"http://{ipAddress}:{port}/");
    server.Start();
    
    // پردازش درخواست‌ها
    while (server.IsListening)
    {
        var context = await server.GetContextAsync();
        await HandleStreamRequestAsync(context);
    }
    
    return true;
}
```

### استریم به تلویزیون (DLNA)

```csharp
public async Task<bool> StartTVStreamingAsync(string deviceAddress)
{
    // جستجوی دستگاه‌های DLNA در شبکه
    var devices = await DlnaFinder.FindDevicesAsync();
    
    // انتخاب دستگاه هدف
    var targetDevice = devices.FirstOrDefault(d => d.Address == deviceAddress);
    
    if (targetDevice != null)
    {
        // ارسال مدیا به دستگاه
        await targetDevice.PlayAsync(_currentTrack.FilePath);
        return true;
    }
    
    return false;
}
```

### استریم به TrafficWatch

```csharp
public async Task StreamToTrafficWatchAsync()
{
    var endpoint = DashboardAddonService.Instance.GetAddonApiEndpoint("music-player");
    
    using (var client = new HttpClient())
    {
        var content = new StreamContent(GetAudioStream());
        await client.PostAsync($"{endpoint}/stream", content);
    }
}
```

---

## تنظیمات و پیکربندی

### تنظیمات پیش‌فرض

در `MusicPlayerAddonInfo.cs`:

```csharp
Settings = new Dictionary<string, object>
{
    { "Language", "fa" },           // fa, en, ar
    { "Volume", 75 },               // 0-100
    { "Shuffle", false },           // true/false
    { "Repeat", "none" },           // none, one, all
    { "ShowVideoAsAudio", true },   // پردازش ویدئو به عنوان صوتی
    { "EnableNetworkStreaming", true },
    { "EnableDLNA", true },
    { "CodecPriority", "auto" }     // auto, high, low
};
```

### ذخیره تنظیمات

```csharp
public void UpdateAddonSettings(string id, Dictionary<string, object> settings)
{
    var addon = GetAddonById(id);
    if (addon != null)
    {
        addon.Settings = settings;
        addon.LastUpdated = DateTime.Now;
        SaveAddonsAsync().Wait();
    }
}
```

---

## نمونه کدها

### نمونه کامل: پخش فایل

```csharp
using MusicPlayerH.Services.Dashboard;
using MusicPlayerH.Models.Dashboard;

public async Task PlayMediaFile(string filePath)
{
    var musicService = new MusicPlayerService();
    musicService.Initialize();
    
    // بررسی وجود فایل
    if (!File.Exists(filePath))
    {
        Console.WriteLine("File not found!");
        return;
    }
    
    // استخراج اطلاعات مدیا
    var track = await musicService.GetMediaInfoAsync(filePath);
    
    // افزودن به لیست پخش
    await musicService.AddToPlaylistAsync(filePath);
    
    // پخش
    await musicService.PlayTrackAsync(track);
    
    Console.WriteLine($"Now playing: {track.Title}");
}
```

### نمونه: ایجاد لیست پخش از پوشه

```csharp
public async Task CreatePlaylistFromFolder(string folderPath)
{
    var musicService = new MusicPlayerService();
    
    string[] extensions = { 
        ".mp3", ".wav", ".flac", ".aac", ".ogg", ".m4a",
        ".mp4", ".mkv", ".avi", ".mov", ".wmv"
    };
    
    var files = Directory.GetFiles(folderPath)
        .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
        .ToArray();
    
    foreach (var file in files)
    {
        await musicService.AddToPlaylistAsync(file);
    }
    
    Console.WriteLine($"Added {files.Length} tracks to playlist.");
}
```

### نمونه: استریم به شبکه

```csharp
public async Task StartStreamingToNetwork()
{
    var musicService = new MusicPlayerService();
    
    // دریافت IP محلی
    string localIp = GetLocalIpAddress();
    int port = 8080;
    
    bool success = await musicService.StartNetworkStreamingAsync(localIp, port);
    
    if (success)
    {
        Console.WriteLine($"Streaming at: http://{localIp}:{port}");
    }
}

private string GetLocalIpAddress()
{
    var host = Dns.GetHostEntry(Dns.GetHostName());
    foreach (var ip in host.AddressList)
    {
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            return ip.ToString();
        }
    }
    return "127.0.0.1";
}
```

---

## عیب‌یابی

### مشکل: فایل پخش نمی‌شود

**راه حل:**
1. بررسی کنید فایل وجود داشته باشد
2. فرمت فایل پشتیبانی شود
3. کدک مناسب نصب باشد
4. دسترسی خواندن به فایل وجود داشته باشد

### مشکل: استریم کار نمی‌کند

**راه حل:**
1. فایروال را بررسی کنید
2. پورت مورد نظر آزاد باشد
3. دستگاه هدف در شبکه باشد
4. پروتکل DLNA فعال باشد

### مشکل: زبان تغییر نمی‌کند

**راه حل:**
1. تنظیمات را ذخیره کنید
2. برنامه را ری‌استارت کنید
3. فایل تنظیمات را بررسی کنید

---

## بهترین روش‌ها

1. **Thread Safety**: بروزرسانی UI فقط در thread اصلی
2. **Error Handling**: مدیریت تمام خطاها
3. **Performance**: استفاده از Async/Await
4. **Memory Management**: Dispose منابع
5. **User Experience**: نمایش وضعیت به کاربر

---

## بیلد و پابلیش

### بیلد Debug

```bash
dotnet build -c Debug
```

### بیلد Release

```bash
dotnet build -c Release
```

### پابلیش برای ویندوز

```bash
dotnet publish -c Release -r win-x64 \
  -p:PublishSingleFile=true \
  -p:SelfContained=false \
  -o ./publish
```

### پابلیش Self-Contained

```bash
dotnet publish -c Release -r win-x64 \
  -p:PublishSingleFile=true \
  -p:SelfContained=true \
  -o ./publish-standalone
```

---

## تماس و پشتیبانی

برای گزارش مشکلات یا提出 پیشنهادات:
- GitHub Issues: https://github.com/hamerstandr/music-player-H/issues
- Email: support@musicplayerh.ir

---

**نسخه سند:** 1.0  
**تاریخ انتشار:** 2024  
**تهیه شده برای:** Music Player H Development Team

</div>

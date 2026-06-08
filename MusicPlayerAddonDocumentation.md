# مستندات افزونه پخش‌کننده موسیقی TrafficWatch

## فهرست مطالب
1. [معرفی](#معرفی)
2. [ویژگی‌ها](#ویژگی‌ها)
3. [معماری](#معماری)
4. [نصب و راه‌اندازی](#نصب-و-راه‌اندازی)
5. [راهنمای استفاده](#راهنمای-استفاده)
6. [تنظیمات](#تنظیمات)
7. [API Reference](#api-reference)
8. [عیب‌یابی](#عیب‌یابی)

---

## معرفی

افزونه پخش‌کننده موسیقی TrafficWatch یک ماژول حرفه‌ای برای پخش فایل‌های صوتی و ویدئویی است که به عنوان بخشی از سیستم داشبورد TrafficWatch طراحی شده است. این افزونه قابلیت‌های پیشرفته‌ای شامل پخش لیست‌های پخش، استریم به شبکه و تلویزیون، و پشتیبانی از فرمت‌های مختلف را فراهم می‌کند.

### اهداف پروژه

- ارائه یک پخش‌کننده موسیقی سبک و بهینه
- پشتیبانی از فرمت‌های صوتی و ویدئویی متنوع
- امکان استریم به دستگاه‌های دیگر در شبکه
- رابط کاربری چندزبانه (فارسی، انگلیسی، عربی)
- یکپارچه‌سازی کامل با سیستم داشبورد TrafficWatch

---

## ویژگی‌ها

### قابلیت‌های اصلی

#### 1. پخش حرفه‌ای
- ✅ پخش/توقف/مکث/ادامه
- ✅ پرش به ترک بعدی/قبلی
- ✅ نوار پیشرفت تعاملی
- ✅ نمایش اطلاعات کامل ترک (عنوان، هنرمند، آلبوم، کاور)

#### 2. لیست پخش پیشرفته
- ✅ افزودن تکی یا گروهی فایل‌ها
- ✅ افزودن کل پوشه‌ها
- ✅ کشیدن و رها کردن (Drag & Drop)
- ✅ مرتب‌سازی بر اساس معیارهای مختلف
- ✅ ذخیره و بارگذاری لیست‌های پخش

#### 3. حالت‌های پخش
- ✅ حالت تصادفی (Shuffle)
- ✅ تکرار تک ترک (Repeat One)
- ✅ تکرار کل لیست (Repeat All)
- ✅ پخش بدون فاصله (Gapless Playback)

#### 4. پشتیبانی از فرمت‌ها

**صوتی:**
- MP3, WAV, FLAC, AAC, OGG Vorbis, M4A, WMA, ALAC

**ویدئویی (به عنوان صوتی):**
- MP4, MKV, AVI, MOV, WMV, FLV, WebM

#### 5. استریمینگ
- ✅ استریم به شبکه محلی (HTTP Streaming)
- ✅ استریم به تلویزیون (DLNA/UPnP)
- ✅ پشتیبانی از Chromecast

#### 6. چندزبانه
- ✅ فارسی (Farsi/Persian)
- ✅ انگلیسی (English)
- ✅ عربی (Arabic)

#### 7. مدیریت کدک
- ✅ تشخیص خودکار کدک
- ✅ اولویت‌بندی کدک‌ها
- ✅ پشتیبانی از K-Lite Codec Pack

---

## معماری

### ساختار فایل‌ها

```
TrafficWatch/
├── Models/Dashboard/
│   ├── AddonInfo.cs                  # کلاس پایه افزونه
│   ├── MusicPlayerAddonInfo.cs       # مدل اطلاعات افزونه موزیک
│   └── MediaTrackInfo.cs             # مدل اطلاعات ترک
│
├── Services/Dashboard/
│   ├── DashboardAddonService.cs      # سرویس مدیریت افزونه‌ها
│   └── MusicPlayerService.cs         # سرویس پخش موسیقی
│
├── View/Dashboard/
│   ├── MusicPlayerTab.xaml           # رابط کاربری
│   └── MusicPlayerTab.xaml.cs        # کد پشت صحنه
│
└── ViewModel/Dashboard/
    └── MusicPlayerViewModel.cs       # ViewModel (MVVM)
```

---

## نصب و راه‌اندازی

### پیش‌نیازها

- .NET Framework 4.7.2 یا بالاتر
- Windows 10/11
- TrafficWatch نسخه 2.0 یا بالاتر

### مراحل نصب

#### 1. افزودن به App.xaml.cs

```csharp
private void Application_Startup(object sender, StartupEventArgs e)
{
    DashboardAddonService.Instance.Initialize();
}
```

#### 2. ثبت افزونه در DashboardAddonService

```csharp
private void RegisterDefaultAddons()
{
    if (!_addons.Any(a => a.Id == "music-player"))
    {
        _addons.Add(new MusicPlayerAddonInfo());
    }
}
```

#### 3. افزودن تب به MainWindow

```csharp
private void CreateAddonTab(AddonInfo addon)
{
    var tabItem = new TabItem { Header = addon.Name, Tag = addon.Id };
    
    var control = addon.Id switch
    {
        "music-player" => new View.Dashboard.MusicPlayerTab(),
        _ => null
    };
    
    if (control != null)
    {
        tabItem.Content = control;
        MainTabControl.Items.Add(tabItem);
    }
}
```

---

## راهنمای استفاده

### افزودن فایل به لیست پخش

1. **دکمه Add Files**: انتخاب تکی یا گروهی فایل‌ها
2. **دکمه Add Folder**: افزودن تمام فایل‌های یک پوشه
3. **Drag & Drop**: کشیدن فایل‌ها به لیست پخش

### کنترل‌های پخش

| دکمه | عملکرد |
|------|--------|
| ▶/⏸ | پخش/مکث |
| ⏹ | توقف |
| ⏭ | ترک بعدی |
| ⏮ | ترک قبلی |
| 🔀 | حالت تصادفی |
| 🔁 | حالت تکرار |
| 📡 | استریم به شبکه/تلویزیون |

### استریم به دستگاه‌های دیگر

1. روی دکمه 📡 کلیک کنید
2. انتخاب کنید: Stream to Network یا Stream to TV
3. دستگاه مقصد را انتخاب کنید
4. پخش شروع می‌شود

---

## تنظیمات

| تنظیم | نوع | پیش‌فرض | توضیحات |
|-------|-----|---------|----------|
| Language | String | "fa" | زبان (fa/en/ar) |
| Volume | Integer | 75 | حجم صدا (0-100) |
| Shuffle | Boolean | false | حالت تصادفی |
| Repeat | String | "none" | تکرار (none/one/all) |
| ShowVideoAsAudio | Boolean | true | پردازش ویدئو به عنوان صوتی |
| EnableNetworkStreaming | Boolean | true | استریم شبکه |
| EnableDLNA | Boolean | true | استریم به تلویزیون |

---

## API Reference

### MusicPlayerService - متدهای اصلی

```csharp
public void Initialize()
public async Task<bool> PlayAsync(string filePath)
public async Task<bool> PlayTrackAsync(MediaTrackInfo track)
public async Task StopAsync()
public async Task PauseAsync()
public async Task ResumeAsync()
public void PlayNext()
public void PlayPrevious()
public async Task AddToPlaylistAsync(string filePath)
public void RemoveFromPlaylist(int index)
public void ClearPlaylist()
public async Task<bool> StartNetworkStreamingAsync(string ipAddress, int port)
public async Task<bool> StartTVStreamingAsync(string deviceAddress)
public void SetVolume(int volume)
public void ToggleShuffle()
public void CycleRepeatMode()
public ObservableCollection<MediaTrackInfo> GetPlaylist()
public MediaTrackInfo GetCurrentTrack()
public PlaybackState GetPlaybackState()
```

### MediaTrackInfo - خصوصیات

```csharp
public string Id { get; set; }
public string Title { get; set; }
public string Artist { get; set; }
public string Album { get; set; }
public int Duration { get; set; }
public string FilePath { get; set; }
public string MediaType { get; set; }
public string Format { get; set; }
public string AudioCodec { get; set; }
public string VideoCodec { get; set; }
public int Bitrate { get; set; }
public int SampleRate { get; set; }
public int Channels { get; set; }
public PlaybackState State { get; set; }
public int CurrentPosition { get; set; }
public byte[] CoverArt { get; set; }
public bool IsStreaming { get; set; }
public string StreamUrl { get; set; }
```

---

## عیب‌یابی

### مشکل: افزونه نمایش داده نمی‌شود

```csharp
var addon = DashboardAddonService.Instance.GetAddonById("music-player");
Console.WriteLine($"Exists: {addon != null}, Enabled: {addon?.IsEnabled}, Installed: {addon?.IsInstalled}");
```

### مشکل: فایل پخش نمی‌شود

- بررسی وجود فایل
- بررسی فرمت فایل
- بررسی نصب کدک‌ها

### مشکل: استریم کار نمی‌کند

```bash
netstat -an | findstr :9091
ping 192.168.1.100
```

---

## کلیدهای میانبر

| کلید | عملکرد |
|------|--------|
| Space | پخش/مکث |
| Ctrl+S | توقف |
| Ctrl+→ | ترک بعدی |
| Ctrl+← | ترک قبلی |
| Ctrl+Up | افزایش صدا |
| Ctrl+Down | کاهش صدا |
| Ctrl+M | قطع صدا |

---

## تماس و پشتیبانی

- **GitHub**: https://github.com/hamerstandr/TrafficWatch/issues
- **Email**: support@trafficwatch.ir

---

**نسخه مستند:** 1.0  
**تاریخ:** 2024  
**وابسته به:** TrafficWatch Music Player Addon v1.0.0

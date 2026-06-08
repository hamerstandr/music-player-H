# TrafficWatch Dashboard v2.0

<div dir="rtl">

## 🎯 درباره پروژه

TrafficWatch یک داشبورد سیستم مانیتورینگ پیشرفته با قابلیت گسترش از طریق افزونه‌ها است. این برنامه به شما امکان می‌دهد دانلودها، موسیقی‌ها و منابع سیستم را در یک محیط واحد مدیریت کنید.

### ✨ ویژگی‌های اصلی

- **سیستم افزونه‌ای قابل گسترش**: اضافه کردن ماژول‌های جدید به سادگی
- **Download Manager**: یکپارچه‌سازی با DownloadMenger2 برای مدیریت دانلودها
- **Music Player**: پخش کننده حرفه‌ای موسیقی و ویدئو با قابلیت استریم
- **System Monitor**: مانیتورینگ CPU، RAM، شبکه و دیسک
- **چندزبانه**: پشتیبانی از فارسی، انگلیسی و عربی
- **UI مدرن و کاربرپسند**: طراحی زیبا و راحت

---

## 📦 نصب و راه‌اندازی

### پیش‌نیازها

- **.NET 6.0 Runtime** (برای نسخه Framework-dependent)
- **Windows 10/11** (x64 یا x86)
- **DownloadMenger2** (اختیاری - برای افزونه دانلود منیجر)

### روش‌های نصب

#### 1. دانلود از GitHub Releases
به صفحه [Releases](https://github.com/hamerstandr/TrafficWatch/releases) بروید و آخرین نسخه را دانلود کنید.

#### 2. ساخت از سورس کد

```bash
# کلون کردن ریپازیتوری
git clone https://github.com/hamerstandr/TrafficWatch.git
cd TrafficWatch

# اجرای اسکریپت بیلد
./build.sh          # برای Linux/macOS
.\build.ps1         # برای Windows PowerShell

# یا استفاده مستقیم از dotnet
dotnet publish -c Release -r win-x64 -o ./publish
```

---

## 🔌 افزونه‌ها

### افزونه‌های پیش‌فرض

| افزونه | شناسه | پورت API | توضیحات |
|--------|-------|----------|---------|
| Download Manager | `download-manager` | 9090 | مدیریت دانلودها با DownloadMenger2 |
| Music Player | `music-player` | 9091 | پخش موسیقی و ویدئو |
| System Monitor | `system-monitor` | - | مانیتورینگ منابع سیستم |

### افزودن افزونه سفارشی

```csharp
var addon = new MyCustomAddonInfo();
DashboardAddonService.Instance.AddCustomAddon(addon);
```

---

## 🎵 Music Player Addon

### ویژگی‌ها

- ✅ پخش فایل‌های صوتی: MP3, WAV, FLAC, AAC, OGG, M4A, WMA, ALAC
- ✅ پخش فایل‌های ویدئویی به عنوان صوتی: MP4, MKV, AVI, MOV, WMV, FLV, WebM
- ✅ لیست پخش پیشرفته با Drag & Drop
- ✅ حالت‌های Shuffle و Repeat
- ✅ استریم به شبکه محلی
- ✅ استریم به تلویزیون (DLNA/Chromecast)
- ✅ پشتیبانی از چندین زبان (فارسی، انگلیسی، عربی)
- ✅ نمایش اطلاعات کامل ترک (ID3 Tags)
- ✅ کنترل_volume، کیفیت صدا

### کلیدهای میانبر

| کلید | عملکرد |
|------|--------|
| Space | پخش/مکث |
| Ctrl+N | ترک بعدی |
| Ctrl+P | ترک قبلی |
| Ctrl+S | توقف |
| Ctrl+O | افزودن فایل |
| Ctrl+F | افزودن پوشه |
| Ctrl+L | پاک کردن لیست |

---

## ⚙️ تنظیمات

### تنظیمات عمومی

در فایل `Settings.settings`:

```xml
<Setting Name="DashboardAddonsEnabled" Type="System.Boolean">True</Setting>
<Setting Name="DashboardRefreshInterval" Type="System.Int32">5</Setting>
<Setting Name="ShowAddonTabs" Type="System.Boolean">True</Setting>
```

### تنظیمات Music Player

```csharp
var addon = DashboardAddonService.Instance.GetAddonById("music-player");
addon.Settings["Language"] = "fa";  // fa, en, ar
addon.Settings["Volume"] = 75;
addon.Settings["Shuffle"] = false;
addon.Settings["Repeat"] = "none";  // none, one, all
addon.Settings["ShowVideoAsAudio"] = true;
addon.Settings["EnableNetworkStreaming"] = true;
addon.Settings["EnableDLNA"] = true;
```

---

## 🔧 توسعه‌دهندگان

### ساختار پروژه

```
TrafficWatch/
├── Models/Dashboard/
│   ├── AddonInfo.cs              # کلاس پایه افزونه
│   ├── MediaTrackInfo.cs         # مدل اطلاعات ترک
│   ├── MusicPlayerAddonInfo.cs   # مدل افزونه موزیک
│   ├── DownloadManagerAddonInfo.cs
│   └── SystemMonitorAddonInfo.cs
│
├── Services/Dashboard/
│   ├── DashboardAddonService.cs  # سرویس مدیریت افزونه‌ها
│   └── MusicPlayerService.cs     # سرویس پخش موسیقی
│
├── View/Dashboard/
│   └── MusicPlayerTab.xaml       # UI موزیک پلیر
│
├── ViewModel/Dashboard/          # ViewModelها
├── Properties/
│   └── AssemblyInfo.cs           # اطلاعات اسمبلی
│
├── TrafficWatch.csproj           # فایل پروژه
├── build.sh                      # اسکریپت بیلد (Linux/macOS)
├── build.ps1                     # اسکریپت بیلد (Windows)
└── README.md                     # این فایل
```

### ایجاد افزونه جدید

1. یک کلاس جدید از `AddonInfo` ایجاد کنید
2. سرویس مربوطه را در `Services/Dashboard` بسازید
3. UI افزونه را در `View/Dashboard` ایجاد کنید
4. افزونه را در `DashboardAddonService.RegisterDefaultAddons()` ثبت کنید

مستندات کامل را در [MusicPlayerAddonDocumentation.md](../MusicPlayerAddonDocumentation.md) و [PublishInstructions.md](PublishInstructions.md) مطالعه کنید.

---

## 📝 تغییرات نسخه 2.0

###新增 features

- ✨ سیستم افزونه‌ای کامل
- 🎵 Music Player حرفه‌ای با استریمینگ
- 🌐 پشتیبانی چندزبانه (FA/EN/AR)
- 📺 استریم به تلویزیون (DLNA)
- 🎼 پشتیبانی از کدک‌های متنوع
- 📋 لیست پخش پیشرفته
- ⚡ بهینه‌سازی عملکرد

### رفع مشکلات

- 🐛 بهبود مدیریت خطاها
- 🐛 بهینه‌سازی مصرف حافظه
- 🐛 بهبود Thread Safety

---

## 🤝 مشارکت

از مشارکت شما استقبال می‌کنیم! لطفاً قبل از ارسال PR موارد زیر را رعایت کنید:

1. کدها را با استانداردهای C# بنویسید
2. تست‌های لازم را اضافه کنید
3. مستندات را بروزرسانی کنید
4. از کامنت‌های فارسی/انگلیسی استفاده کنید

---

## 📄 لایسنس

این پروژه تحت لایسنس MIT منتشر شده است.

---

## 📞 تماس و پشتیبانی

- **GitHub Issues**: https://github.com/hamerstandr/TrafficWatch/issues
- **Email**: support@trafficwatch.ir
- **وبسایت**: https://trafficwatch.ir

---

**تهیه شده با ❤️ توسط تیم TrafficWatch**  
**نسخه:** 2.0.0  
**تاریخ:** 2024

</div>

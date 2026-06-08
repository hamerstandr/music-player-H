# 🎵 Music Player H - موزیک پلیر حرفه‌ای

<div dir="rtl">

## معرفی

**Music Player H** یک پخش‌کننده موسیقی حرفه‌ای و مدرن برای ویندوز است که با فناوری WPF و C# توسعه یافته است. این برنامه با طراحی کاربرپسند و امکانات پیشرفته، تجربه‌ای بی‌نظیر از پخش موسیقی را ارائه می‌دهد.

---

## ✨ ویژگی‌های کلیدی

### 🎶 پخش حرفه‌ای
- پخش/توقف/مکث/ادامه
- پرش به ترک بعدی/قبلی
- نوار پیشرفت تعاملی
- نمایش اطلاعات کامل ترک (عنوان، هنرمند، آلبوم، کاور)
- کنترل حجم صدا با اسلایدر دقیق

### 📚 لیست پخش پیشرفته
- افزودن تکی یا گروهی فایل‌ها
- افزودن کل پوشه‌ها
- Drag & Drop
- مدیریت کامل لیست پخش (حذف، جابجایی، مرتب‌سازی)
- نمایش زمان کل و تعداد ترک‌ها

### 🔄 حالت‌های پخش
- **Shuffle**: پخش تصادفی
- **Repeat One**: تکرار تک ترک
- **Repeat All**: تکرار کل لیست

### 🎬 پشتیبانی از فرمت‌ها
#### فرمت‌های صوتی:
- MP3, WAV, FLAC, AAC, OGG, M4A, WMA, ALAC

#### فرمت‌های ویدئویی (به عنوان صوتی):
- MP4, MKV, AVI, MOV, WMV, FLV, WebM

### 🌐 استریمینگ
- استریم به شبکه محلی (HTTP Streaming)
- استریم به تلویزیون (DLNA/Chromecast)
- پشتیبانی از پروتکل‌های UPnP

### 🌍 چندزبانه
- **فارسی** (Farsi/Persian) 🇮🇷
- **انگلیسی** (English) 🇺🇸
- **عربی** (Arabic) 🇸🇦

### ⚙️ تنظیمات کامل
- انتخاب زبان رابط کاربری
- تنظیم کیفیت صدا
- مدیریت کدک‌ها
- شخصی‌سازی ظاهر
- تنظیمات شبکه و استریم

---

## 🚀 نصب و راه‌اندازی

### پیش‌نیازها
- ویندوز 10 یا بالاتر
- .NET 6.0 Runtime (یا نصب خودکار)
- حداقل 256MB RAM
- 500MB فضای ذخیره‌سازی

### نصب
1. دانلود فایل نصبی `music-player-H-setup.exe`
2. اجرای فایل نصبی
3. دنبال کردن مراحل نصب
4. اجرای برنامه از منوی Start

---

## 💻 راهنمای استفاده

### شروع سریع
1. برنامه را اجرا کنید
2. روی دکمه **"افزودن فایل"** کلیک کنید
3. فایل‌های موسیقی مورد نظر را انتخاب کنید
4. از پخش موسیقی لذت ببرید!

### کلیدهای میانبر
| کلید | عملکرد |
|------|--------|
| Space | پخش/توقف |
| Ctrl+P | مکث/ادامه |
| Ctrl+N | ترک بعدی |
| Ctrl+B | ترک قبلی |
| Ctrl+S | توقف کامل |
| Ctrl+Shuffle | فعال/غیرفعال Shuffle |
| Ctrl+R | تغییر حالت تکرار |
| F11 | تمام صفحه |
| Ctrl++ | افزایش حجم |
| Ctrl+- | کاهش حجم |
| Ctrl+0 | تنظیم حجم به 100% |

---

## 📁 ساختار پروژه

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
├── Resources/                        # فایل‌های منابع
│
└── music-player-H.csproj             # فایل پروژه
```

---

## 🔧 بیلد و کامپایل

### با Visual Studio 2022
1. باز کردن `music-player-H.csproj` در Visual Studio
2. انتخاب Configuration: **Release**
3. انتخاب Platform: **x64**
4. منوی Build > Build Solution (Ctrl+Shift+B)

### با خط فرمان
```bash
cd music-player-H
dotnet restore
dotnet build -c Release
```

---

## 📦 پابلیش

### پابلیش سریع
```bash
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -o ./publish
```

### انواع پابلیش

| نوع | حجم | نیاز به .NET | توصیه |
|-----|-----|--------------|--------|
| Framework-dependent | ~10MB | ✅ دارد | عمومی |
| Single File | ~15MB | ✅ دارد | ⭐ توصیه شده |
| Self-Contained | ~70MB | ❌ ندارد | سیستم‌های بدون .NET |

---

## 🛠️ تکنولوژی‌های استفاده شده

- **زبان برنامه‌نویسی:** C# 10
- **فریم‌ورک:** .NET 6.0
- **رابط کاربری:** WPF (Windows Presentation Foundation)
- **پخش صوتی:** NAudio
- **متادیتا:** TagLibSharp
- **JSON:** Newtonsoft.Json

---

## 📝 مجوز

© 2024 Music Player H Team  
تمام حقوق محفوظ است.

---

## 🤝 مشارکت

برای گزارش مشکلات یا پیشنهاد ویژگی‌های جدید:
- ایمیل: support@musicplayerh.ir
- GitHub Issues

---

## 📞 پشتیبانی

- وب‌سایت: https://musicplayerh.ir
- ایمیل: support@musicplayerh.ir

</div>

---

**نسخه:** 2.0.0  
**تاریخ انتشار:** 2024  
**تهیه شده توسط:** Music Player H Team

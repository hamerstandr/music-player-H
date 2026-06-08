# راهنمای پابلیش پروژه TrafficWatch

## پیش‌نیازها

1. **.NET 6.0 SDK** یا بالاتر نصب باشد
2. **Visual Studio 2022** یا **VS Code** با افزونه C#
3. دسترسی به اینترنت برای دانلود NuGet packages

## مراحل پابلیش

### روش 1: استفاده از خط فرمان (توصیه شده)

#### پابلیش برای ویندوز (x64)

```bash
cd /workspace/TrafficWatch

# پابلیش در حالت Release
dotnet publish -c Release -r win-x64 --self-contained false -o ./publish/win-x64

# پابلیش به صورت Single File
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ./publish/win-x64-single

# پابلیش کامل با تمام وابستگی‌ها (Self-Contained)
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish/win-x64-full
```

#### پابلیش برای ویندوز (x86)

```bash
dotnet publish -c Release -r win-x86 --self-contained false -o ./publish/win-x86
```

#### پابلیش برای ARM64

```bash
dotnet publish -c Release -r win-arm64 --self-contained false -o ./publish/win-arm64
```

### روش 2: استفاده از Visual Studio

1. پروژه را در Visual Studio باز کنید
2. روی پروژه راست‌کلیک کرده و **Publish** را انتخاب کنید
3. **Folder** را به عنوان Target انتخاب کنید
4. تنظیمات زیر را اعمال کنید:
   - Configuration: **Release**
   - Runtime: **win-x64**
   - Deployment Mode: **Framework-dependent** (کوچک‌تر) یا **Self-contained** (بزرگ‌تر اما مستقل)
   - Single File: **Enabled** (اختیاری)
5. روی **Publish** کلیک کنید

## خروجی پابلیش

پس از پابلیش موفق، فایل‌های زیر در پوشه `publish` ایجاد می‌شوند:

```
publish/
├── win-x64/
│   ├── TrafficWatch.exe          # فایل اجرایی اصلی
│   ├── TrafficWatch.dll          # کتابخانه اصلی
│   ├── *.dll                     # وابستگی‌ها
│   └── appsettings.json          # تنظیمات
├── win-x64-single/
│   └── TrafficWatch.exe          # فایل اجرایی تک‌فایلی
└── win-x64-full/
    └── TrafficWatch.exe          # فایل اجرایی کامل با تمام وابستگی‌ها
```

## اندازه فایل‌های خروجی (تقریبی)

| نوع پابلیش | اندازه تقریبی | توضیحات |
|------------|---------------|----------|
| Framework-dependent | ~5-10 MB | نیاز به نصب .NET 6 دارد |
| Single File | ~15-20 MB | تمام وابستگی‌ها در یک فایل |
| Self-contained | ~60-80 MB | مستقل کامل، بدون نیاز به نصب چیزی |

## نصب پیش‌نیازها روی سیستم کاربر

اگر از پابلیش Framework-dependent استفاده می‌کنید، کاربر باید .NET 6 Desktop Runtime را نصب کند:

### دانلود از مایکروسافت
- لینک مستقیم: https://aka.ms/dotnet-core-applaunch?missing_runtime=true&arch=x64&rid=win10-x64&apphost_version=6.0.0
- یا جستجوی ".NET 6 Desktop Runtime x64" در گوگل

## ساخت Installer (اختیاری)

### استفاده از WiX Toolset

```bash
# نصب WiX Toolset
dotnet tool install --global wix

# ساخت فایل MSI
wix build -platform x64 -output publish/installer/TrafficWatch.msi installer.wxs
```

### استفاده از Inno Setup

1. Inno Setup Compiler را نصب کنید
2. اسکریپت زیر را به عنوان `installer.iss` ذخیره کنید:

```ini
[Setup]
AppName=TrafficWatch
AppVersion=2.0.0
DefaultDirName={pf}\TrafficWatch
DefaultGroupName=TrafficWatch
OutputDir=publish\installer

[Files]
Source: "publish\win-x64\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{group}\TrafficWatch"; Filename: "{app}\TrafficWatch.exe"
Name: "{autodesktop}\TrafficWatch"; Filename: "{app}\TrafficWatch.exe"
```

3. کامپایل کنید:
```bash
iscc installer.iss
```

## تنظیمات نهایی قبل از پابلیش

### 1. بررسی نسخه
فایل `TrafficWatch.csproj` را بررسی کنید:
```xml
<Version>2.0.0</Version>
```

### 2. افزودن آیکون برنامه
```xml
<ApplicationIcon>Resources\app.ico</ApplicationIcon>
```

### 3. تنظیمات AssemblyInfo
فایل `Properties\AssemblyInfo.cs` را ایجاد یا ویرایش کنید:

```csharp
using System.Reflection;

[assembly: AssemblyTitle("TrafficWatch")]
[assembly: AssemblyDescription("Advanced system monitoring dashboard")]
[assembly: AssemblyCompany("TrafficWatch Team")]
[assembly: AssemblyProduct("TrafficWatch Dashboard")]
[assembly: AssemblyCopyright("© 2024 TrafficWatch Team")]
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]
```

## تست پس از پابلیش

1. فایل اجرایی را در یک محیط تمیز تست کنید
2. تمام افزونه‌ها را بررسی کنید:
   - Download Manager
   - Music Player
   - System Monitor
3. تنظیمات زبان را تست کنید (فارسی، انگلیسی، عربی)
4. قابلیت استریم را بررسی کنید
5. لیست پخش را تست کنید

## عیب‌یابی

### خطای "Runtime not found"
```bash
# نصب runtime مورد نیاز
dotnet --list-runtimes
# یا دانلود از سایت مایکروسافت
```

### خطای وابستگی‌ها
```bash
# پاک کردن کش و بازیابی مجدد
dotnet clean
dotnet restore
dotnet publish -c Release
```

### حجم زیاد فایل خروجی
- از PublishReadyToRun استفاده نکنید
- از Self-contained=false استفاده کنید
- وابستگی‌های غیرضروری را حذف کنید

## انتشار در GitHub Releases

1. فایل‌های پابلیش شده را Zip کنید:
```bash
cd publish/win-x64
zip -r ../../TrafficWatch-v2.0.0-win-x64.zip *
```

2. به GitHub Releases بروید
3. تگ جدید ایجاد کنید (مثلاً v2.0.0)
4. فایل Zip را آپلود کنید
5. توضیحات نسخه را بنویسید

## چک‌لیست نهایی

- [ ] بیلد در حالت Release بدون خطا
- [ ] تمام تست‌ها پاس شدند
- [ ] فایل csproj نسخه درست دارد
- [ ] مستندات بروزرسانی شدند
- [ ] فایل‌های پابلیش تست شدند
- [ ] Installer ساخته شد (اختیاری)
- [ ] در GitHub Releases آپلود شد

---

**تهیه شده برای:** TrafficWatch Development Team  
**تاریخ:** 2024  
**نسخه سند:** 1.0

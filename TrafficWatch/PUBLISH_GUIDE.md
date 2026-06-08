# 🚀 راهنمای جامع پابلیش TrafficWatch v2.0

## 📋 فهرست مطالب
1. [پیش‌نیازها](#پیش‌نیازها)
2. [بررسی نهایی پروژه](#بررسی-نهایی-پروژه)
3. [مراحل پابلیش در Visual Studio](#مراحل-پابلیش-در-visual-studio)
4. [پابلیش با خط فرمان](#پابلیش-با-خط-فرمان)
5. [تنظیمات پیشرفته](#تنظیمات-پیشرفته)
6. [ساخت Installer](#ساخت-installer)
7. [تست و کنترل کیفیت](#تست-و-کنترل-کیفیت)
8. [انتشار در GitHub](#انتشار-در-github)
9. [عیب‌یابی](#عیب‌یابی)

---

## ✅ پیش‌نیازها

### نرم‌افزارهای مورد نیاز:
- **Visual Studio 2022** (نسخه 17.0 یا بالاتر) با workloadهای:
  - .NET Desktop Development
  - WPF Development Tools
  
- **.NET 6.0 SDK** (یا بالاتر)
  - دانلود از: https://dotnet.microsoft.com/download/dotnet/6.0
  
- **Git** (برای Version Control)
  - دانلود از: https://git-scm.com/download/win

### سخت‌افزار پیشنهادی:
- RAM: حداقل 8GB
- فضای دیسک: حداقل 5GB فضای خالی
- ویندوز 10/11 (x64)

---

## 🔍 بررسی نهایی پروژه

### قبل از پابلیش، موارد زیر را بررسی کنید:

#### 1. ساختار فایل‌ها
```
TrafficWatch/
├── Models/Dashboard/
│   ├── AddonInfo.cs              ✅
│   ├── DownloadManagerAddonInfo.cs ✅
│   ├── MediaTrackInfo.cs         ✅
│   ├── MusicPlayerAddonInfo.cs   ✅
│   └── SystemMonitorAddonInfo.cs ✅
├── Services/Dashboard/
│   ├── DashboardAddonService.cs  ✅
│   └── MusicPlayerService.cs     ✅
├── View/Dashboard/
│   ├── MusicPlayerTab.xaml       ✅
│   └── MusicPlayerTab.xaml.cs    ✅
├── ViewModel/Dashboard/          📁
├── Properties/
│   ├── AssemblyInfo.cs           ✅
│   └── Settings.settings         ⚠️ (بررسی شود)
├── Resources/                    📁
├── TrafficWatch.csproj           ✅ (بروزرسانی شد)
├── App.xaml                      ⚠️ (بررسی شود)
├── App.xaml.cs                   ⚠️ (بررسی شود)
├── MainWindow.xaml               ⚠️ (بررسی شود)
└── MainWindow.xaml.cs            ⚠️ (بررسی شود)
```

#### 2. چک‌لیست کدها

**App.xaml.cs:**
```csharp
private void Application_Startup(object sender, StartupEventArgs e)
{
    // ✅ باید این خط وجود داشته باشد
    DashboardAddonService.Instance.Initialize();
    
    // ✅ بررسی وضعیت افزونه‌ها
    _ = CheckInstalledAddonsAsync();
}
```

**MainWindow.xaml.cs:**
```csharp
// ✅ باید تب‌های افزونه‌ها را لود کند
private void LoadAddonTabs()
{
    var addons = DashboardAddonService.Instance.GetAllAddons();
    foreach (var addon in addons.Where(a => a.IsEnabled && a.IsInstalled))
    {
        CreateAddonTab(addon);
    }
}
```

#### 3. بررسی فایل csproj
- ✅ نسخه: 2.0.0
- ✅ TargetFramework: net6.0-windows
- ✅ PublishSingleFile: true
- ✅ EnableCompressionInSingleFile: true
- ✅ ApplicationIcon: Resources\app.ico

---

## 💻 مراحل پابلیش در Visual Studio

### روش 1: Publish Wizard (توصیه شده برای مبتدیان)

#### مرحله 1: باز کردن پروژه
1. Visual Studio 2022 را باز کنید
2. `File > Open > Project/Solution`
3. فایل `TrafficWatch.csproj` را انتخاب کنید

#### مرحله 2: شروع فرآیند Publish
1. در Solution Explorer، روی پروژه **TrafficWatch** راست‌کلیک کنید
2. گزینه **Publish...** را انتخاب کنید

#### مرحله 3: انتخاب Target
1. **Folder** را انتخاب کنید
2. روی **Next** کلیک کنید

#### مرحله 4: تنظیمات Folder
1. مسیر خروجی را مشخص کنید (پیشنهاد: `./publish/win-x64`)
2. روی **Finish** کلیک کنید

#### مرحله 5: پیکربندی پروفایل
روی **Settings** کلیک کرده و تنظیمات زیر را اعمال کنید:

| تنظیم | مقدار پیشنهادی | توضیحات |
|-------|----------------|----------|
| Configuration | Release | بهینه‌سازی کامل |
| Runtime | win-x64 | برای ویندوز 64 بیتی |
| Deployment Mode | Framework-dependent | حجم کمتر (~10MB) |
| Single File | ✅ Enabled | یک فایل اجرایی |
| ReadyToRun | ✅ Enabled | اجرای سریع‌تر |
| Trim unused assemblies | ❌ Disabled | ممکن است باعث مشکل شود |

#### مرحله 6: Publish
1. روی **Save** کلیک کنید
2. دکمه **Publish** را بزنید
3. صبر کنید تا فرآیند کامل شود

### روش 2: استفاده از Package Manager Console

1. در Visual Studio: `Tools > NuGet Package Manager > Package Manager Console`
2. دستورات زیر را اجرا کنید:

```powershell
# پاک‌سازی
dotnet clean

# بازیابی پکیج‌ها
dotnet restore

# بیلد در حالت Release
dotnet build -c Release

# پابلیش معمولی
dotnet publish -c Release -r win-x64 --self-contained false -o ./publish/win-x64

# پابلیش به صورت Single File (توصیه شده)
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -o ./publish/win-x64-single

# پابلیش کامل Self-Contained (بدون نیاز به نصب .NET)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish/win-x64-full
```

---

## 🖥️ پابلیش با خط فرمان (CMD/PowerShell)

### اسکریپت PowerShell خودکار

فایل `build.ps1` را اجرا کنید:

```powershell
cd /workspace/TrafficWatch
.\build.ps1 -Configuration Release -Runtime win-x64 -SingleFile
```

### پارامترهای اسکریپت:

| پارامتر | مقدار پیش‌فرض | توضیحات |
|---------|---------------|----------|
| `-Configuration` | Release | Debug یا Release |
| `-Runtime` | win-x64 | win-x64, win-x86, win-arm64 |
| `-SelfContained` | False | شامل تمام وابستگی‌ها |
| `-SingleFile` | False | یک فایل اجرایی |
| `-OutputPath` | ./publish | مسیر خروجی |

### نمونه‌های مختلف:

```powershell
# پابلیش معمولی
.\build.ps1

# پابلیش Self-Contained کامل
.\build.ps1 -SelfContained

# پابلیش برای ARM64
.\build.ps1 -Runtime win-arm64

# پابلیش با همه گزینه‌ها
.\build.ps1 -Configuration Release -Runtime win-x64 -SelfContained -SingleFile -OutputPath ./dist
```

---

## ⚙️ تنظیمات پیشرفته

### 1. افزودن آیکون برنامه

یک فایل آیکون بسازید یا دانلود کنید:
- فرمت: `.ico`
- سایز پیشنهادی: 256x256 پیکسل
- مسیر: `Resources/app.ico`

```xml
<!-- در TrafficWatch.csproj -->
<ApplicationIcon>Resources\app.ico</ApplicationIcon>
```

### 2. تنظیمات AssemblyVersion

در فایل `Properties/AssemblyInfo.cs`:

```csharp
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]
[assembly: AssemblyInformationalVersion("2.0.0+release")]
```

### 3. ایجاد فایل Settings.settings

1. در Solution Explorer راست‌کلیک > `Add > New Item`
2. `Settings File` را انتخاب کنید
3. نام را `Settings.settings` بگذارید

تنظیمات پیش‌فرض:

| نام | نوع | مقدار پیش‌فرض | Scope |
|-----|-----|---------------|-------|
| DashboardAddonsEnabled | bool | True | User |
| DashboardRefreshInterval | int | 5 | User |
| MusicPlayerLanguage | string | fa-IR | User |
| MusicPlayerVolume | double | 0.7 | User |
| EnableNetworkStreaming | bool | True | User |

### 4. بهینه‌سازی حجم خروجی

```xml
<!-- در csproj اضافه کنید -->
<PropertyGroup>
  <PublishTrimmed>false</PublishTrimmed>
  <PublishReadyToRunComposite>true</PublishReadyToRunComposite>
  <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
</PropertyGroup>
```

---

## 📦 ساخت Installer

### روش 1: WiX Toolset (حرفه‌ای)

#### نصب WiX:
```powershell
dotnet tool install --global wix
```

#### ایجاد فایل installer.wxs:
```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Product Id="*" Name="TrafficWatch" Language="1033" 
           Version="2.0.0" Manufacturer="TrafficWatch Team" 
           UpgradeCode="PUT-GUID-HERE">
    
    <Package InstallerVersion="200" Compressed="yes" 
             InstallScope="perMachine" />
    
    <MajorUpgrade DowngradeErrorMessage="A newer version is already installed." />
    
    <MediaTemplate EmbedCab="yes" />
    
    <Feature Id="ProductFeature" Title="TrafficWatch" Level="1">
      <ComponentGroupRef Id="ProductComponents" />
      <ComponentRef Id="ApplicationComponent" />
    </Feature>
    
    <UIRef Id="WixUI_Minimal" />
  </Product>

  <Fragment>
    <Directory Id="TARGETDIR" Name="SourceDir">
      <Directory Id="ProgramFilesFolder">
        <Directory Id="INSTALLFOLDER" Name="TrafficWatch" />
      </Directory>
      <Directory Id="ProgramMenuFolder">
        <Directory Id="ApplicationProgramsFolder" Name="TrafficWatch"/>
      </Directory>
    </Directory>
  </Fragment>

  <Fragment>
    <ComponentGroup Id="ProductComponents" Directory="INSTALLFOLDER">
      <Component Id="ApplicationComponent" Directory="INSTALLFOLDER">
        <File Id="TrafficWatchExe" Source="$(var.TrafficWatchPath)\TrafficWatch.exe" 
              KeyPath="yes" Checksum="yes">
          <Shortcut Id="StartMenuShortcut" Directory="ApplicationProgramsFolder" 
                    Name="TrafficWatch" WorkingDirectory="INSTALLFOLDER" />
        </File>
      </Component>
    </ComponentGroup>
  </Fragment>
</Wix>
```

#### بیلد MSI:
```powershell
wix build -platform x64 -output publish/installer/TrafficWatch-v2.0.0.msi installer.wxs
```

### روش 2: Inno Setup (ساده‌تر)

#### نصب Inno Setup:
- دانلود از: https://jrsoftware.org/isdl.php

#### ایجاد فایل installer.iss:
```ini
[Setup]
AppName=TrafficWatch
AppVersion=2.0.0
AppPublisher=TrafficWatch Team
DefaultDirName={pf}\TrafficWatch
DefaultGroupName=TrafficWatch
OutputDir=publish\installer
OutputBaseFilename=TrafficWatch-v2.0.0-Setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "persian"; MessagesFile: "compiler:Languages\Persian.isl"

[Files]
Source: "publish\win-x64\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs

[Icons]
Name: "{group}\TrafficWatch"; Filename: "{app}\TrafficWatch.exe"
Name: "{autodesktop}\TrafficWatch"; Filename: "{app}\TrafficWatch.exe"
Name: "{autostart}\TrafficWatch"; Filename: "{app}\TrafficWatch.exe"

[Run]
Filename: "{app}\TrafficWatch.exe"; Description: "{cm:LaunchProgram,TrafficWatch}"; Flags: nowait postinstall skipifsilent
```

#### کامپایل:
```powershell
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" installer.iss
```

---

## 🧪 تست و کنترل کیفیت

### چک‌لیست تست قبل از انتشار:

#### 1. تست عملکردی
- [ ] برنامه بدون خطا اجرا می‌شود
- [ ] تمام تب‌ها لود می‌شوند
- [ ] موزیک پلیر فایل‌ها را پخش می‌کند
- [ ] لیست پخش کار می‌کند
- [ ] تنظیمات زبان تغییر می‌کند (FA/EN/AR)
- [ ] استریم به شبکه فعال است
- [ ] دانلود منیجر متصل می‌شود
- [ ] مانیتور سیستم اطلاعات نشان می‌دهد

#### 2. تست UI/UX
- [ ] رابط کاربری روان است
- [ ] فونت‌ها درست نمایش داده می‌شوند
- [ ] RTL برای فارسی و عربی کار می‌کند
- [ ] آیکون‌ها درست هستند
- [ ] نوار پیشرفت کار می‌کند

#### 3. تست Performance
- [ ] زمان لود زیر 3 ثانیه
- [ ] مصرف RAM منطقی (<200MB)
- [ ] CPU Usage پایین (<5% در حالت idle)
- [ ] بدون Memory Leak

#### 4. تست Compatibility
- [ ] ویندوز 10 (x64)
- [ ] ویندوز 11 (x64)
- [ ] ویندوز سرور 2019/2022
- [ ] بدون نصب .NET (نسخه Self-Contained)

### ابزارهای تست:

```powershell
# تست با dotnet test (اگر Unit Test دارید)
dotnet test -c Release

# تست استاتیک کد
dotnet format --verify-no-changes

# آنالیز وابستگی‌ها
dotnet list package --include-transitive
```

---

## 🌐 انتشار در GitHub Releases

### مرحله 1: آماده‌سازی فایل‌ها

```powershell
# Zip کردن خروجی
cd publish/win-x64-single
Compress-Archive -Path * -DestinationPath ../../TrafficWatch-v2.0.0-win-x64.zip -Force

# ایجاد checksum
Get-FileHash ../../TrafficWatch-v2.0.0-win-x64.zip | Format-List > ../../CHECKSUMS.txt
```

### مرحله 2: ایجاد Release Notes

فایل `RELEASE_NOTES.md`:
```markdown
# TrafficWatch v2.0.0

## 🎉 ویژگی‌های جدید

### Music Player Addon
- ✅ پخش حرفه‌ای فایل‌های صوتی و ویدئویی
- ✅ لیست پخش پیشرفته با Drag & Drop
- ✅ پشتیبانی از فرمت‌های متعدد (MP3, FLAC, WAV, MP4, MKV, ...)
- ✅ حالت‌های Shuffle و Repeat
- ✅ استریم به شبکه و تلویزیون (DLNA/Chromecast)
- ✅ چندزبانه (فارسی، انگلیسی، عربی)

### سیستم افزونه‌ای
- ✅ مدیریت آسان افزونه‌ها
- ✅ فعال/غیرفعال کردن هر افزونه
- ✅ تغییر ترتیب نمایش
- ✅ API برای ارتباط با برنامه‌های خارجی

### بهبودهای کلی
- ✅ بهینه‌سازی Performance
- ✅ رفع باگ‌های گزارش شده
- ✅ بهبود UI/UX

## 📦 نصب

1. فایل ZIP را دانلود کنید
2. از حالت فشرده خارج کنید
3. فایل `TrafficWatch.exe` را اجرا کنید

### پیش‌نیازها
- ویندوز 10/11 (x64)
- .NET 6 Desktop Runtime (برای نسخه Framework-dependent)

## 🔗 لینک‌ها

- [مستندات کامل](docs/MusicPlayerAddonDocumentation.md)
- [راهنمای پابلیش](docs/PUBLISH_GUIDE.md)
- [گزارش باگ](https://github.com/hamerstandr/TrafficWatch/issues)

## 📊 آمار

- حجم فایل: ~15MB (Single File)
- تعداد خطوط کد: ~1800
- تعداد افزونه‌ها: 3

---
**تاریخ انتشار:** 2024-06-08  
**توسعه‌دهنده:** TrafficWatch Team
```

### مرحله 3: آپلود در GitHub

1. به ریپازیتوری GitHub بروید
2. تب **Releases** > **Draft a new release**
3. تگ جدید: `v2.0.0`
4. عنوان: `TrafficWatch v2.0.0 - Music Player Addon`
5. توضیحات: محتوای RELEASE_NOTES.md را کپی کنید
6. فایل‌ها را آپلود کنید:
   - TrafficWatch-v2.0.0-win-x64.zip
   - CHECKSUMS.txt
   - TrafficWatch-v2.0.0-Setup.exe (اگر ساختید)
7. ✅ **Set as the latest release**
8. **Publish release**

---

## 🔧 عیب‌یابی

### مشکل 1: خطای "Resource not found"

**علت:** فایل آیکون وجود ندارد  
**راه حل:**
```xml
<!-- موقتاً از csproj حذف کنید -->
<!-- <ApplicationIcon>Resources\app.ico</ApplicationIcon> -->
```

### مشکل 2: خطای وابستگی‌ها

```powershell
# پاک کردن کش
dotnet clean
dotnet restore --force

# حذف پوشه obj و bin
Remove-Item -Recurse -Force obj, bin
```

### مشکل 3: حجم زیاد فایل خروجی

**راه حل‌ها:**
1. از `--self-contained false` استفاده کنید
2. `PublishReadyToRun=false` تنظیم کنید
3. وابستگی‌های غیرضروری را حذف کنید

### مشکل 4: خطای XAML Compilation

```powershell
# بررسی syntax XAML
dotnet build /p:DebugType=none

# غیرفعال کردن XAML Compiler موقت
dotnet publish -p:MarkupCompilePass1=false
```

### مشکل 5: برنامه اجرا نمی‌شود

**بررسی Event Viewer:**
1. `Win + R` > `eventvwr.msc`
2. Windows Logs > Application
3. خطاهای مربوط به TrafficWatch را بررسی کنید

**اجرا از خط فرمان:**
```cmd
TrafficWatch.exe --debug
```

---

## 📊 مقایسه انواع پابلیش

| نوع | حجم | نیاز به .NET | سرعت اجرا | توصیه |
|-----|-----|--------------|-----------|--------|
| Framework-dependent | ~10MB | ✅ دارد | خوب | ✅ عمومی |
| Single File | ~15MB | ✅ دارد | بسیار خوب | ✅ توصیه شده |
| Self-Contained | ~70MB | ❌ ندارد | عالی | برای سیستم‌های بدون .NET |
| AOT Compiled | ~50MB | ❌ ندارد | بهترین | 🔄 آزمایشی |

---

## ✅ چک‌لیست نهایی قبل از انتشار

- [ ] بیلد بدون خطا و Warning
- [ ] تمام تست‌ها پاس شدند
- [ ] نسخه در csproj و AssemblyInfo یکسان است
- [ ] مستندات بروزرسانی شدند
- [ ] آیکون برنامه اضافه شد
- [ ] فایل‌های پابلیش تست شدند
- [ ] Installer ساخته شد (اختیاری)
- [ ] Release Notes نوشته شد
- [ ] در GitHub آپلود شد
- [ ] به کاربران اطلاع‌رسانی شد

---

## 🎉 موفق باشید!

پروژه TrafficWatch v2.0 آماده انتشار است. برای سوالات و پشتیبانی:
- 📧 Email: support@trafficwatch.ir
- 💬 GitHub Issues: https://github.com/hamerstandr/TrafficWatch/issues
- 📚 مستندات: `/docs`

**تهیه شده توسط:** TrafficWatch Development Team  
**تاریخ:** 2024-06-08  
**نسخه سند:** 2.0

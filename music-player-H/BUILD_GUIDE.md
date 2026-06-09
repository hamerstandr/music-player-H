# 🛠️ راهنمای بیلد و پابلیش Music Player H

<div dir="rtl">

## فهرست مطالب

1. [پیش‌نیازها](#پیش‌نیازها)
2. [بیلد در Visual Studio 2026](#بیلد-در-visual-studio-2026)
3. [بیلد با خط فرمان](#بیلد-با-خط-فرمان)
4. [پابلیش برای انتشار](#پابلیش-برای-انتشار)
5. [تنظیمات Advanced .NET 10](#تنظیمات-advanced-net-10)
6. [عیب‌یابی](#عیب‌یابی)

---

## پیش‌نیازها

### نرم‌افزارهای مورد نیاز

1. **Visual Studio 2026** (Community, Professional یا Enterprise)
   - دانلود از: https://visualstudio.microsoft.com/
   - **نکته:** حداقل نسخه 17.12 یا بالاتر required است
   
2. **Workloadهای لازم**:
   - ✅ .NET Desktop Development
   
3. **.NET 10 SDK**
   - معمولاً با Visual Studio 2026 نصب می‌شود
   - دانلود جداگانه: https://dotnet.microsoft.com/download/dotnet/10.0

### بررسی نصب بودن

```bash
# بررسی نسخه dotnet
dotnet --version

# باید خروجی مشابه دهد: 10.0.x
```

---

## بیلد در Visual Studio 2026

### مرحله 1: باز کردن پروژه

1. Visual Studio 2026 را باز کنید
2. از منوی File > Open > Project/Solution
3. فایل `music-player-H.csproj` را انتخاب کنید

### مرحله 2: تنظیم Configuration

1. از نوار ابزار بالا:
   - **Configuration**: Release (برای انتشار) یا Debug (برای توسعه)
   - **Platform**: x64

### مرحله 3: Restore Dependencies

1. از منوی Build > Clean Solution
2. سپس Build > Rebuild Solution

یا کلید میانبر: `Ctrl+Shift+B`

### مرحله 4: بررسی خطاها

- پنجره Output را بررسی کنید
- باید پیام "Build succeeded" را ببینید

---

## بیلد با خط فرمان

### بیلد ساده

```bash
cd music-player-H
dotnet build
```

### بیلد Debug

```bash
dotnet build -c Debug
```

### بیلد Release

```bash
dotnet build -c Release
```

### بیلد با جزئیات کامل

```bash
dotnet build -c Release -v detailed
```

---

## پابلیش برای انتشار

### پابلیش Framework-Dependent (توصیه شده)

این نوع پابلیش کوچکترین حجم را دارد اما نیاز به نصب .NET 10 روی سیستم کاربر دارد:

```bash
dotnet publish -c Release -r win-x64 \
  -p:PublishSingleFile=true \
  -p:SelfContained=false \
  -p:PublishTrimmed=true \
  -p:EnableCompressionInSingleFile=true \
  -o ./publish
```

**خروجی:**
- حجم: ~8-12 MB (با فشرده‌سازی)
- نیاز به .NET 10: ✅ دارد
- فایل خروجی: `publish/music-player-H.exe`

### پابلیش Self-Contained

این نوع پابلیش تمام وابستگی‌ها را شامل می‌شود و نیازی به نصب .NET ندارد:

```bash
dotnet publish -c Release -r win-x64 \
  -p:PublishSingleFile=true \
  -p:SelfContained=true \
  -p:PublishTrimmed=true \
  -p:EnableCompressionInSingleFile=true \
  -o ./publish-standalone
```

**خروجی:**
- حجم: ~50-70 MB (با Trim و فشرده‌سازی)
- نیاز به .NET 10: ❌ ندارد
- فایل خروجی: `publish-standalone/music-player-H.exe`

### پابلیش با فشرده‌سازی پیشرفته

برای کاهش حداکثری حجم فایل نهایی:

```bash
dotnet publish -c Release -r win-x64 \
  -p:PublishSingleFile=true \
  -p:SelfContained=false \
  -p:PublishTrimmed=true \
  -p:TrimMode=link \
  -p:EnableCompressionInSingleFile=true \
  -p:ReadyToRun=true \
  -o ./publish-optimized
```

---

## تنظیمات Advanced .NET 10

### ویژگی‌های جدید .NET 10

پروژه از ویژگی‌های پیشرفته .NET 10 بهره می‌برد:

```xml
<!-- در فایل .csproj -->
<LangVersion>preview</LangVersion>
<EnableWindowsTargeting>true</EnableWindowsTargeting>
<TrimMode>link</TrimMode>
<PublishTrimmed>true</PublishTrimmed>
```

### بهینه‌سازی‌های WPF در .NET 10

- ✅ بهبود Performance در رندرینگ UI
- ✅ کاهش مصرف حافظه
- ✅ پشتیبانی از HiDPI و 4K
- ✅ بهبود انیمیشن‌ها و ترنزیشن‌ها

### Trim Settings

برای حذف کدهای استفاده نشده و کاهش حجم:

```bash
dotnet publish -c Release -r win-x64 \
  -p:PublishTrimmed=true \
  -p:TrimMode=link \
  -p:TrimmerDefaultAction=link \
  -o ./publish-trimmed
```

### ReadyToRun Compilation

برای بهبود سرعت اجرا:

```bash
dotnet publish -c Release -r win-x64 \
  -p:PublishReadyToRun=true \
  -p:ReadyToRunComposite=true \
  -o ./publish-r2r
```

---

## اسکریپت‌های اتوماتیک

### PowerShell Script برای پابلیش کامل

فایل `publish.ps1`:

```powershell
# publish.ps1
param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [switch]$SelfContained,
    [switch]$Optimized
)

Write-Host "🎵 Building Music Player H with .NET 10..." -ForegroundColor Green

# Clean
dotnet clean

# Restore
dotnet restore

# Build
dotnet build -c $Configuration

# Publish
$outputDir = if ($SelfContained) { "./publish-standalone" } else { "./publish" }
$selfContainedFlag = if ($SelfContained) { "-p:SelfContained=true" } else { "-p:SelfContained=false" }
$trimFlag = if ($Optimized) { "-p:PublishTrimmed=true -p:TrimMode=link" } else { "" }
$r2rFlag = if ($Optimized) { "-p:PublishReadyToRun=true" } else { "" }

Write-Host "📦 Publishing to $outputDir..." -ForegroundColor Cyan

dotnet publish -c $Configuration -r $Runtime `
  -p:PublishSingleFile=true `
  $selfContainedFlag `
  $trimFlag `
  $r2rFlag `
  -p:EnableCompressionInSingleFile=true `
  -o $outputDir

Write-Host "✅ Publish completed!" -ForegroundColor Green
Write-Host "📁 Output: $outputDir/music-player-H.exe" -ForegroundColor Cyan

# Show file size
$fileSize = (Get-Item "$outputDir/music-player-H.exe").Length / 1MB
Write-Host "📊 File Size: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Yellow
```

**اجرا:**
```powershell
.\publish.ps1 -Configuration Release
.\publish.ps1 -Configuration Release -SelfContained
.\publish.ps1 -Configuration Release -Optimized
```

### Bash Script برای Linux/Mac

فایل `build.sh`:

```bash
#!/bin/bash

echo "🎵 Building Music Player H (.NET 10)..."

# Clean
dotnet clean

# Restore
dotnet restore

# Build
dotnet build -c Release

# Publish
echo "📦 Publishing optimized build..."
dotnet publish -c Release -r win-x64 \
  -p:PublishSingleFile=true \
  -p:SelfContained=false \
  -p:PublishTrimmed=true \
  -p:TrimMode=link \
  -p:EnableCompressionInSingleFile=true \
  -p:PublishReadyToRun=true \
  -o ./publish

echo "✅ Build completed!"
echo "📁 Output: ./publish/music-player-H.exe"

# Show file size
if command -v du &> /dev/null; then
    du -h ./publish/music-player-H.exe
fi
```

**اجرا:**
```bash
chmod +x build.sh
./build.sh
```

---

## عیب‌یابی

### خطا: .NET 10 not found

**راه حل:**
```bash
# نصب .NET 10 SDK
# Windows: دانلود از https://dotnet.microsoft.com/download/dotnet/10.0
# یا با winget:
winget install Microsoft.DotNet.SDK.10
```

### خطا: Visual Studio version too old

**راه حل:**
- Visual Studio 2026 نسخه 17.12 یا بالاتر نصب کنید
- یا از Visual Studio 2022 با آخرین آپدیت استفاده کنید

### خطا: NuGet packages not restoring

**راه حل:**
```bash
# پاک کردن کش NuGet
dotnet nuget locals all --clear

# Force restore
dotnet restore --force
```

### خطا: Build fails with missing references

**راه حل:**
1. بررسی کنید تمام PackageReferenceها در `.csproj` وجود دارند
2. اجرای دستی:
   ```bash
   dotnet add package NAudio --version 2.2.1
   dotnet add package TagLibSharp --version 2.3.0
   dotnet add package Newtonsoft.Json --version 13.0.3
   ```

### خطا: Publish creates multiple files

**راه حل:**
مطمئن شوید `-p:PublishSingleFile=true` را اضافه کرده‌اید

### حجم فایل خیلی بزرگ است

**راه حل:**
- از `-p:EnableCompressionInSingleFile=true` استفاده کنید
- از Trimmed publish استفاده کنید:
  ```bash
  -p:PublishTrimmed=true -p:TrimMode=link
  ```
- از ReadyToRun استفاده نکنید (حجم را افزایش می‌دهد)

### خطا: Trim warnings

**راه حل:**
برخی کتابخانه‌ها ممکن است با Trim سازگار نباشند. می‌توانید:
```bash
-p:SuppressTrimAnalysisWarnings=true
```

---

## مقایسه انواع پابلیش (.NET 10)

| نوع | حجم | سرعت اجرا | نیاز به .NET 10 | توصیه برای |
|-----|-----|-----------|----------------|------------|
| Framework-Dependent | 8-12MB | بسیار سریع | ✅ دارد | کاربران عمومی |
| Self-Contained | 50-70MB | بسیار سریع | ❌ ندارد | سیستم‌های بدون .NET |
| Single File + Trim | 30-40MB | سریع | ❌ ندارد | توزیع آسان |
| Optimized (R2R+Trim) | 35-45MB | فوق‌العاده سریع | ❌ ندارد | بهترین Performance |

---

## نکات مهم

1. **همیشه قبل از پابلیش نهایی، تست کنید**
2. **از Configuration Release استفاده کنید**
3. **برنامه را روی سیستم‌های مختلف تست کنید** (ویندوز 10, 11, 12)
4. **فایل PDB را برای دیباگ نگه دارید** (در صورت نیاز)
5. **ورژن برنامه را در AssemblyInfo.cs بروزرسانی کنید**
6. **از .NET 10 Features استفاده کنید** (LangVersion=preview)

---

## چک‌لیست قبل از انتشار

- [ ] بیلد بدون خطا و هشدار
- [ ] تمام ویژگی‌ها تست شده‌اند
- [ ] ورژن برنامه بروزرسانی شده
- [ ] README بروزرسانی شده
- [ ] فایل‌های اضافی حذف شده‌اند
- [ ] روی ویندوز 10، 11 و 12 تست شده
- [ ] فایل اجرایی کار می‌کند
- [ ] یکپارچگی با TrafficWatch بررسی شده
- [ ] تنظیمات چندزبانه تست شده

---

**نسخه سند:** 2.0  
**تاریخ:** 2024  
**فریم‌ورک:** .NET 10.0  
**IDE:** Visual Studio 2026  
**تهیه شده برای:** Music Player H Development Team

</div>

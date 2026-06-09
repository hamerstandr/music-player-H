# 🛠️ راهنمای بیلد و پابلیش Music Player H

<div dir="rtl">

## فهرست مطالب

1. [پیش‌نیازها](#پیش‌نیازها)
2. [بیلد در Visual Studio](#بیلد-در-visual-studio)
3. [بیلد با خط فرمان](#بیلد-با-خط-فرمان)
4. [پابلیش برای انتشار](#پابلیش-برای-انتشار)
5. [عیب‌یابی](#عیب‌یابی)

---

## پیش‌نیازها

### نرم‌افزارهای مورد نیاز

1. **Visual Studio 2022** (Community, Professional یا Enterprise)
   - دانلود از: https://visualstudio.microsoft.com/
   
2. **Workloadهای لازم**:
   - ✅ .NET Desktop Development
   
3. **.NET 6.0 SDK**
   - معمولاً با Visual Studio نصب می‌شود
   - دانلود جداگانه: https://dotnet.microsoft.com/download/dotnet/6.0

### بررسی نصب بودن

```bash
# بررسی نسخه dotnet
dotnet --version

# باید خروجی مشابه دهد: 6.0.x
```

---

## بیلد در Visual Studio

### مرحله 1: باز کردن پروژه

1. Visual Studio 2022 را باز کنید
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

این نوع پابلیش کوچکترین حجم را دارد اما نیاز به نصب .NET 6 روی سیستم کاربر دارد:

```bash
dotnet publish -c Release -r win-x64 \
  -p:PublishSingleFile=true \
  -p:SelfContained=false \
  -o ./publish
```

**خروجی:**
- حجم: ~10-15 MB
- نیاز به .NET 6: ✅ دارد
- فایل خروجی: `publish/music-player-H.exe`

### پابلیش Self-Contained

این نوع پابلیش تمام وابستگی‌ها را شامل می‌شود و نیازی به نصب .NET ندارد:

```bash
dotnet publish -c Release -r win-x64 \
  -p:PublishSingleFile=true \
  -p:SelfContained=true \
  -p:PublishTrimmed=true \
  -o ./publish-standalone
```

**خروجی:**
- حجم: ~60-80 MB
- نیاز به .NET 6: ❌ ندارد
- فایل خروجی: `publish-standalone/music-player-H.exe`

### پابلیش با فشرده‌سازی

برای کاهش حجم فایل نهایی:

```bash
dotnet publish -c Release -r win-x64 \
  -p:PublishSingleFile=true \
  -p:SelfContained=false \
  -p:EnableCompressionInSingleFile=true \
  -o ./publish-compressed
```

---

## تنظیمات Advanced

### ایجاد Installer (MSI)

برای ساخت فایل نصبی:

1. نصب Extension: **WiX Toolset Visual Studio Extension**
2. افزودن پروژه Setup به Solution
3. تنظیمات را مطابق زیر انجام دهید:

```xml
<!-- در فایل .wxs -->
<Product Id="*" Name="Music Player H" Language="1033" Version="2.0.0" Manufacturer="Music Player H Team" UpgradeCode="*">
  <Package InstallerVersion="200" Compressed="yes" InstallScope="perMachine" />
  
  <MajorUpgrade DowngradeErrorMessage="A newer version of [ProductName] is already installed." />
  
  <Feature Id="ProductFeature" Title="Music Player H" Level="1">
    <ComponentGroupRef Id="ProductComponents" />
    <ComponentRef Id="ApplicationComponent" />
  </Feature>
</Product>
```

### امضای دیجیتال

برای امضای فایل اجرایی:

```bash
signtool sign /f certificate.pfx /p password /t http://timestamp.digicert.com publish/music-player-H.exe
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
    [switch]$SelfContained
)

Write-Host "Building Music Player H..." -ForegroundColor Green

# Clean
dotnet clean

# Restore
dotnet restore

# Build
dotnet build -c $Configuration

# Publish
$outputDir = if ($SelfContained) { "./publish-standalone" } else { "./publish" }
$selfContainedFlag = if ($SelfContained) { "-p:SelfContained=true" } else { "-p:SelfContained=false" }

dotnet publish -c $Configuration -r $Runtime `
  -p:PublishSingleFile=true `
  $selfContainedFlag `
  -p:EnableCompressionInSingleFile=true `
  -o $outputDir

Write-Host "Publish completed to $outputDir" -ForegroundColor Green
Write-Host "Output file: $outputDir/music-player-H.exe" -ForegroundColor Cyan
```

**اجرا:**
```powershell
.\publish.ps1 -Configuration Release
.\publish.ps1 -Configuration Release -SelfContained
```

### Bash Script برای Linux/Mac

فایل `build.sh`:

```bash
#!/bin/bash

echo "🎵 Building Music Player H..."

# Clean
dotnet clean

# Restore
dotnet restore

# Build
dotnet build -c Release

# Publish
echo "📦 Publishing..."
dotnet publish -c Release -r win-x64 \
  -p:PublishSingleFile=true \
  -p:SelfContained=false \
  -p:EnableCompressionInSingleFile=true \
  -o ./publish

echo "✅ Build completed!"
echo "📁 Output: ./publish/music-player-H.exe"
```

**اجرا:**
```bash
chmod +x build.sh
./build.sh
```

---

## عیب‌یابی

### خطا: .NET 6 not found

**راه حل:**
```bash
# نصب .NET 6 SDK
# Windows: دانلود از https://dotnet.microsoft.com/download/dotnet/6.0
# یا با winget:
winget install Microsoft.DotNet.SDK.6
```

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
   dotnet add package NAudio
   dotnet add package TagLibSharp
   dotnet add package Newtonsoft.Json
   ```

### خطا: Publish creates multiple files

**راه حل:**
مطمئن شوید `-p:PublishSingleFile=true` را اضافه کرده‌اید

### حجم فایل خیلی بزرگ است

**راه حل:**
- از `-p:EnableCompressionInSingleFile=true` استفاده کنید
- یا از Trimmed publish استفاده کنید:
  ```bash
  -p:PublishTrimmed=true
  ```

---

## مقایسه انواع پابلیش

| نوع | حجم | سرعت اجرا | نیاز به .NET | توصیه برای |
|-----|-----|-----------|--------------|------------|
| Framework-Dependent | 10-15MB | سریع | ✅ دارد | کاربران عمومی |
| Self-Contained | 60-80MB | سریع | ❌ ندارد | سیستم‌های بدون .NET |
| Single File | 15-20MB | متوسط | ✅ دارد | توزیع آسان |
| Trimmed | 30-40MB | سریع | ❌ ندارد | بهینه‌سازی حجم |

---

## نکات مهم

1. **همیشه قبل از پابلیش نهایی، تست کنید**
2. **از Configuration Release استفاده کنید**
3. **برنامه را روی سیستم‌های مختلف تست کنید**
4. **فایل PDB را برای دیباگ نگه دارید**
5. **ورژن برنامه را در AssemblyInfo.cs بروزرسانی کنید**

---

## چک‌لیست قبل از انتشار

- [ ] بیلد بدون خطا
- [ ] تمام ویژگی‌ها تست شده‌اند
- [ ] ورژن برنامه بروزرسانی شده
- [ ] README بروزرسانی شده
- [ ] فایل‌های اضافی حذف شده‌اند
- [ ] روی ویندوز 10 و 11 تست شده
- [ ] فایل نصبی/اجرایی کار می‌کند

---

**نسخه سند:** 1.0  
**تاریخ:** 2024  
**تهیه شده برای:** Music Player H Development Team

</div>

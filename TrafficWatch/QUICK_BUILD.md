# 🚀 راهنمای سریع بیلد و پابلیش

## ⚡ سریع‌ترین روش (3 دستور)

```powershell
cd TrafficWatch
dotnet restore
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ./publish
```

خروجی در پوشه `publish` خواهد بود.

---

## 📋 مراحل کامل برای Visual Studio

### 1️⃣ باز کردن پروژه
- Visual Studio 2022 را باز کنید
- فایل `TrafficWatch.csproj` را انتخاب کنید

### 2️⃣ بیلد اولیه
```
Build > Build Solution (Ctrl+Shift+B)
```
مطمئن شوید خطایی وجود ندارد.

### 3️⃣ پابلیش
```
Right-click on Project > Publish...
```

تنظیمات پیشنهادی:
- **Configuration:** Release
- **Runtime:** win-x64
- **Deployment:** Framework-dependent
- **Single File:** ✅ Enabled

### 4️⃣ کلیک روی Publish
صبر کنید تا فرآیند کامل شود.

---

## 🎯 انواع پابلیش

### معمولی (پیشنهادی)
```bash
dotnet publish -c Release -r win-x64 -o ./publish
```
حجم: ~10MB | نیاز به .NET 6

### Single File
```bash
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -o ./publish-single
```
حجم: ~15MB | نیاز به .NET 6

### Self-Contained (کامل)
```bash
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish-full
```
حجم: ~70MB | بدون نیاز به نصب چیزی

---

## ✅ چک‌لیست قبل از پابلیش

- [ ] Visual Studio 2022 نصب است
- [ ] .NET 6 SDK نصب است
- [ ] پروژه بدون خطا بیلد می‌شود
- [ ] فایل‌های XAML بررسی شدند
- [ ] نسخه برنامه 2.0.0 است

---

## 🔍 تست سریع

بعد از پابلیش:
```bash
cd publish
.\TrafficWatch.exe
```

برنامه باید اجرا شود و تب‌های زیر را نشان دهد:
- 🎵 Music Player
- ⬇️ Download Manager  
- 📊 System Monitor

---

## ❌ رفع مشکلات رایج

### خطای "SDK not found"
```bash
dotnet --list-sdks
```
اگر خالی بود، از https://dotnet.microsoft.com/download دانلود کنید.

### خطای وابستگی‌ها
```bash
dotnet clean
dotnet restore --force
dotnet build
```

### خطای XAML
فایل‌های `.xaml` را باز کرده و مطمئن شوید syntax درست است.

---

## 📞 پشتیبانی

مشکلی داشتید؟
- 📚 مستندات کامل: `PUBLISH_GUIDE.md`
- 💬 Issues: https://github.com/hamerstandr/TrafficWatch/issues

**موفق باشید!** 🎉

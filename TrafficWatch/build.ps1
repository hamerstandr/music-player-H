# اسکریپت پابلیش خودکار TrafficWatch
# این اسکریپت برای PowerShell طراحی شده است

param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [switch]$SelfContained,
    [switch]$SingleFile,
    [string]$OutputPath = "./publish"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  TrafficWatch Build & Publish Script  " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# بررسی وجود .NET SDK
Write-Host "Checking .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: .NET SDK not found!" -ForegroundColor Red
    Write-Host "Please install .NET 6.0 SDK from: https://dotnet.microsoft.com/download" -ForegroundColor Red
    exit 1
}
Write-Host ".NET SDK Version: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# پاک‌سازی پروژه
Write-Host "Cleaning project..." -ForegroundColor Yellow
dotnet clean
if ($LASTEXITCODE -ne 0) {
    Write-Host "Warning: Clean failed, continuing..." -ForegroundColor Yellow
}
Write-Host ""

# بازیابی پکیج‌ها
Write-Host "Restoring packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Package restore failed!" -ForegroundColor Red
    exit 1
}
Write-Host "Packages restored successfully" -ForegroundColor Green
Write-Host ""

# ساخت پارامترهای پابلیش
$publishArgs = @(
    "publish"
    "-c", $Configuration
    "-r", $Runtime
)

if ($SelfContained) {
    $publishArgs += "--self-contained", "true"
    Write-Host "Mode: Self-Contained (Full)" -ForegroundColor Cyan
} else {
    $publishArgs += "--self-contained", "false"
    Write-Host "Mode: Framework-Dependent" -ForegroundColor Cyan
}

if ($SingleFile) {
    $publishArgs += "-p:PublishSingleFile=true"
    Write-Host "Option: Single File Enabled" -ForegroundColor Cyan
}

$publishArgs += "-o", "$OutputPath/$Runtime"

# نمایش تنظیمات
Write-Host ""
Write-Host "Build Configuration: $Configuration" -ForegroundColor Cyan
Write-Host "Runtime: $Runtime" -ForegroundColor Cyan
Write-Host "Output Path: $OutputPath/$Runtime" -ForegroundColor Cyan
Write-Host ""

# اجرای پابلیش
Write-Host "Publishing..." -ForegroundColor Yellow
Write-Host ""
dotnet @publishArgs

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Error: Publish failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Publish completed successfully!      " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Output location: $OutputPath/$Runtime" -ForegroundColor Cyan
Write-Host ""

# نمایش حجم فایل‌ها
Write-Host "Output files:" -ForegroundColor Yellow
Get-ChildItem "$OutputPath/$Runtime" | ForEach-Object {
    $size = "{0:N2}" -f ($_.Length / 1MB)
    Write-Host "  $($_.Name) - ${size} MB" -ForegroundColor Gray
}

$totalSize = (Get-ChildItem "$OutputPath/$Runtime" | Measure-Object -Property Length -Sum).Sum
Write-Host ""
Write-Host "Total size: $("{0:N2}" -f ($totalSize / 1MB)) MB" -ForegroundColor Cyan
Write-Host ""

# ایجاد فایل نسخه
$version = "2.0.0"
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
@"
TrafficWatch Release
====================
Version: $version
Build Date: $timestamp
Runtime: $Runtime
Configuration: $Configuration
"@ | Out-File -FilePath "$OutputPath/$Runtime/RELEASE_INFO.txt" -Encoding UTF8

Write-Host "Release info saved to RELEASE_INFO.txt" -ForegroundColor Green
Write-Host ""
Write-Host "Ready to distribute!" -ForegroundColor Green

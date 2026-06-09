# publish.ps1 - اسکریپت پابلیش Music Player H
param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [switch]$SelfContained,
    [switch]$Compress
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  🎵 Music Player H - Publish Script  " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Clean
Write-Host "[1/4] Cleaning..." -ForegroundColor Yellow
dotnet clean
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Clean failed!" -ForegroundColor Red
    exit 1
}

# Restore
Write-Host "[2/4] Restoring packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Restore failed!" -ForegroundColor Red
    exit 1
}

# Build
Write-Host "[3/4] Building $Configuration..." -ForegroundColor Yellow
dotnet build -c $Configuration --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}

# Publish
$outputDir = if ($SelfContained) { "./publish-standalone" } else { "./publish" }
$selfContainedFlag = if ($SelfContained) { "-p:SelfContained=true" } else { "-p:SelfContained=false" }
$compressFlag = if ($Compress) { "-p:EnableCompressionInSingleFile=true" } else { "" }

Write-Host "[4/4] Publishing to $outputDir..." -ForegroundColor Yellow
Write-Host "   Configuration: $Configuration" -ForegroundColor Gray
Write-Host "   Runtime: $Runtime" -ForegroundColor Gray
Write-Host "   Self-Contained: $SelfContained" -ForegroundColor Gray
Write-Host "   Compression: $Compress" -ForegroundColor Gray
Write-Host ""

dotnet publish -c $Configuration -r $Runtime `
  -p:PublishSingleFile=true `
  $selfContainedFlag `
  $compressFlag `
  -o $outputDir

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  ✅ Publish completed successfully!  " -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "📁 Output directory: $outputDir" -ForegroundColor Cyan
    Write-Host "📄 Output file: $outputDir/music-player-H.exe" -ForegroundColor Cyan
    
    # نمایش حجم فایل
    $exePath = Join-Path $outputDir "music-player-H.exe"
    if (Test-Path $exePath) {
        $fileSize = (Get-Item $exePath).Length / 1MB
        Write-Host "📦 File size: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Cyan
    }
} else {
    Write-Host ""
    Write-Host "❌ Publish failed!" -ForegroundColor Red
    exit 1
}

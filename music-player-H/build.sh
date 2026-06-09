#!/bin/bash
# build.sh - اسکریپت بیلد Music Player H (.NET 10) برای Linux/Mac

echo "========================================"
echo "  🎵 Music Player H - .NET 10 Build    "
echo "========================================"
echo ""

CONFIGURATION="Release"
RUNTIME="win-x64"
SELF_CONTAINED=false
OPTIMIZED=false
COMPRESS=true

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --debug)
            CONFIGURATION="Debug"
            shift
            ;;
        --self-contained)
            SELF_CONTAINED=true
            shift
            ;;
        --optimized)
            OPTIMIZED=true
            shift
            ;;
        --no-compress)
            COMPRESS=false
            shift
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

# Clean
echo "[1/4] Cleaning..."
dotnet clean
if [ $? -ne 0 ]; then
    echo "❌ Clean failed!"
    exit 1
fi

# Restore
echo "[2/4] Restoring packages..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "❌ Restore failed!"
    exit 1
fi

# Build
echo "[3/4] Building $CONFIGURATION..."
dotnet build -c $CONFIGURATION --no-restore
if [ $? -ne 0 ]; then
    echo "❌ Build failed!"
    exit 1
fi

# Publish
if [ "$SELF_CONTAINED" = true ]; then
    OUTPUT_DIR="./publish-standalone"
    SELF_CONTAINED_FLAG="-p:SelfContained=true"
elif [ "$OPTIMIZED" = true ]; then
    OUTPUT_DIR="./publish-optimized"
    SELF_CONTAINED_FLAG="-p:SelfContained=false"
else
    OUTPUT_DIR="./publish"
    SELF_CONTAINED_FLAG="-p:SelfContained=false"
fi

# Set optimized flags
TRIM_FLAG=""
R2R_FLAG=""
if [ "$OPTIMIZED" = true ]; then
    TRIM_FLAG="-p:PublishTrimmed=true -p:TrimMode=link"
    R2R_FLAG="-p:PublishReadyToRun=true"
fi

# Set compression flag
COMPRESS_FLAG=""
if [ "$COMPRESS" = true ] || [ "$OPTIMIZED" = true ]; then
    COMPRESS_FLAG="-p:EnableCompressionInSingleFile=true"
fi

echo "[4/4] Publishing to $OUTPUT_DIR..."
echo "   Configuration: $CONFIGURATION"
echo "   Runtime: $RUNTIME"
echo "   Self-Contained: $SELF_CONTAINED"
echo "   Optimized: $OPTIMIZED"
echo "   Compression: $([ "$COMPRESS" = true ] || [ "$OPTIMIZED" = true ] && echo "true" || echo "false")"
echo ""

dotnet publish -c $CONFIGURATION -r $RUNTIME \
  -p:PublishSingleFile=true \
  $SELF_CONTAINED_FLAG \
  $TRIM_FLAG \
  $R2R_FLAG \
  $COMPRESS_FLAG \
  -o $OUTPUT_DIR

if [ $? -eq 0 ]; then
    echo ""
    echo "========================================"
    echo "  ✅ Publish completed successfully!  "
    echo "========================================"
    echo ""
    echo "📁 Output directory: $OUTPUT_DIR"
    echo "📄 Output file: $OUTPUT_DIR/music-player-H.exe"

    # Show file size if available
    if [ -f "$OUTPUT_DIR/music-player-H.exe" ]; then
        FILE_SIZE=$(ls -lh "$OUTPUT_DIR/music-player-H.exe" | awk '{print $5}')
        echo "📦 File size: $FILE_SIZE"
        
        # Show detailed size in MB
        FILE_SIZE_BYTES=$(stat -c%s "$OUTPUT_DIR/music-player-H.exe" 2>/dev/null || stat -f%z "$OUTPUT_DIR/music-player-H.exe" 2>/dev/null)
        if [ -n "$FILE_SIZE_BYTES" ]; then
            FILE_SIZE_MB=$(echo "scale=2; $FILE_SIZE_BYTES / 1048576" | bc)
            echo "📊 Detailed size: ${FILE_SIZE_MB} MB"
        fi
    fi
    
    echo ""
    echo "🚀 Ready to run with .NET 10!"
else
    echo ""
    echo "❌ Publish failed!"
    exit 1
fi

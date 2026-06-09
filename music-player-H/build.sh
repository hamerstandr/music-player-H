#!/bin/bash
# build.sh - اسکریپت بیلد Music Player H برای Linux/Mac

echo "========================================"
echo "  🎵 Music Player H - Build Script    "
echo "========================================"
echo ""

CONFIGURATION="Release"
RUNTIME="win-x64"
SELF_CONTAINED=false
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
else
    OUTPUT_DIR="./publish"
    SELF_CONTAINED_FLAG="-p:SelfContained=false"
fi

if [ "$COMPRESS" = true ]; then
    COMPRESS_FLAG="-p:EnableCompressionInSingleFile=true"
else
    COMPRESS_FLAG=""
fi

echo "[4/4] Publishing to $OUTPUT_DIR..."
echo "   Configuration: $CONFIGURATION"
echo "   Runtime: $RUNTIME"
echo "   Self-Contained: $SELF_CONTAINED"
echo "   Compression: $COMPRESS"
echo ""

dotnet publish -c $CONFIGURATION -r $RUNTIME \
  -p:PublishSingleFile=true \
  $SELF_CONTAINED_FLAG \
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
    fi
else
    echo ""
    echo "❌ Publish failed!"
    exit 1
fi

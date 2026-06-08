#!/bin/bash
# اسکریپت پابلیش خودکار TrafficWatch
# این اسکریپت برای Linux/macOS طراحی شده است

CONFIGURATION="Release"
RUNTIME="win-x64"
SELF_CONTAINED=false
SINGLE_FILE=false
OUTPUT_PATH="./publish"

# رنگ‌ها
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}  TrafficWatch Build & Publish Script  ${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""

# بررسی وجود .NET SDK
echo -e "${YELLOW}Checking .NET SDK...${NC}"
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}Error: .NET SDK not found!${NC}"
    echo -e "${RED}Please install .NET 6.0 SDK from: https://dotnet.microsoft.com/download${NC}"
    exit 1
fi
DOTNET_VERSION=$(dotnet --version)
echo -e "${GREEN}.NET SDK Version: $DOTNET_VERSION${NC}"
echo ""

# پاک‌سازی پروژه
echo -e "${YELLOW}Cleaning project...${NC}"
dotnet clean || echo -e "${YELLOW}Warning: Clean failed, continuing...${NC}"
echo ""

# بازیابی پکیج‌ها
echo -e "${YELLOW}Restoring packages...${NC}"
if ! dotnet restore; then
    echo -e "${RED}Error: Package restore failed!${NC}"
    exit 1
fi
echo -e "${GREEN}Packages restored successfully${NC}"
echo ""

# ساخت پارامترهای پابلیش
PUBLISH_ARGS="publish -c $CONFIGURATION -r $RUNTIME --self-contained false"

if [ "$SELF_CONTAINED" = true ]; then
    PUBLISH_ARGS="$PUBLISH_ARGS --self-contained true"
    echo -e "${CYAN}Mode: Self-Contained (Full)${NC}"
else
    echo -e "${CYAN}Mode: Framework-Dependent${NC}"
fi

if [ "$SINGLE_FILE" = true ]; then
    PUBLISH_ARGS="$PUBLISH_ARGS -p:PublishSingleFile=true"
    echo -e "${CYAN}Option: Single File Enabled${NC}"
fi

PUBLISH_ARGS="$PUBLISH_ARGS -o $OUTPUT_PATH/$RUNTIME"

# نمایش تنظیمات
echo ""
echo -e "${CYAN}Build Configuration: $CONFIGURATION${NC}"
echo -e "${CYAN}Runtime: $RUNTIME${NC}"
echo -e "${CYAN}Output Path: $OUTPUT_PATH/$RUNTIME${NC}"
echo ""

# اجرای پابلیش
echo -e "${YELLOW}Publishing...${NC}"
echo ""
if ! dotnet $PUBLISH_ARGS; then
    echo ""
    echo -e "${RED}Error: Publish failed!${NC}"
    exit 1
fi

echo ""
echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}  Publish completed successfully!      ${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""
echo -e "${CYAN}Output location: $OUTPUT_PATH/$RUNTIME${NC}"
echo ""

# نمایش حجم فایل‌ها
echo -e "${YELLOW}Output files:${NC}"
if command -v du &> /dev/null; then
    TOTAL_SIZE=0
    for file in "$OUTPUT_PATH/$RUNTIME"/*; do
        if [ -f "$file" ]; then
            SIZE=$(du -h "$file" | cut -f1)
            echo "  $(basename "$file") - $SIZE"
        fi
    done
    TOTAL=$(du -sh "$OUTPUT_PATH/$RUNTIME" | cut -f1)
    echo ""
    echo -e "${CYAN}Total size: $TOTAL${NC}"
fi
echo ""

# ایجاد فایل نسخه
VERSION="2.0.0"
TIMESTAMP=$(date +"%Y-%m-%d %H:%M:%S")
cat > "$OUTPUT_PATH/$RUNTIME/RELEASE_INFO.txt" << RELEASE
TrafficWatch Release
====================
Version: $VERSION
Build Date: $TIMESTAMP
Runtime: $RUNTIME
Configuration: $CONFIGURATION
RELEASE

echo -e "${GREEN}Release info saved to RELEASE_INFO.txt${NC}"
echo ""
echo -e "${GREEN}Ready to distribute!${NC}"

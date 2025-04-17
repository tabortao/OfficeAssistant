#!/bin/bash

# 参数说明
ARCH=$1          # 架构名称 (macos-x64 或 macos-arm64)
SOURCE_PATH=$2   # 构建输出路径
VERSION=$3       # 版本号

# 设置变量
APP_NAME="OfficeAssistant"
DMG_NAME="${APP_NAME}-${ARCH}-v${VERSION}"
APP_DIR="${APP_NAME}.app"
CONTENTS_DIR="${APP_DIR}/Contents"
RESOURCES_DIR="${CONTENTS_DIR}/Resources"
MACOS_DIR="${CONTENTS_DIR}/MacOS"

# 清理旧文件
rm -rf "${APP_DIR}"
rm -f "${DMG_NAME}.dmg"

# 创建应用程序包结构
mkdir -p "${MACOS_DIR}"
mkdir -p "${RESOURCES_DIR}"

# 复制构建文件到 MacOS 目录
cp -r "${SOURCE_PATH}"/* "${MACOS_DIR}/"

# 复制图标
cp "OfficeAssistant/Resources/icon.icns" "${RESOURCES_DIR}/AppIcon.icns" || true

# 创建 Info.plist
cat > "${CONTENTS_DIR}/Info.plist" << EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleDevelopmentRegion</key>
    <string>English</string>
    <key>CFBundleDisplayName</key>
    <string>Office Assistant</string>
    <key>CFBundleExecutable</key>
    <string>OfficeAssistant</string>
    <key>CFBundleIconFile</key>
    <string>AppIcon</string>
    <key>CFBundleIconName</key>
    <string>AppIcon</string>
    <key>CFBundleIdentifier</key>
    <string>top.sdgarden.officeassistant</string>
    <key>CFBundleName</key>
    <string>OfficeAssistant</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleShortVersionString</key>
    <string>${VERSION}</string>
    <key>CFBundleVersion</key>
    <string>${VERSION}</string>
    <key>LSMinimumSystemVersion</key>
    <string>11.0</string>
    <key>CSResourcesFileMapped</key>
    <true/>
    <key>NSHighResolutionCapable</key>
    <true/>
</dict>
</plist>
EOF

# 设置执行权限
chmod +x "${MACOS_DIR}/OfficeAssistant"

# 创建临时目录用于 DMG
TMP_DIR=$(mktemp -d)
cp -r "${APP_DIR}" "${TMP_DIR}/"

# 创建 DMG
create-dmg \
    --volname "Office Assistant" \
    --volicon "OfficeAssistant/Resources/icon.icns" \
    --window-pos 200 120 \
    --window-size 800 400 \
    --icon-size 100 \
    --icon "${APP_NAME}.app" 200 190 \
    --hide-extension "${APP_NAME}.app" \
    --app-drop-link 600 185 \
    --no-internet-enable \
    "${DMG_NAME}.dmg" \
    "${TMP_DIR}/${APP_DIR}"

# 清理临时目录
rm -rf "${TMP_DIR}"
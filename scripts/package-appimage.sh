#!/bin/bash

# 安装依赖
sudo apt-get update -y
sudo apt-get install -y libfuse2 desktop-file-utils

# 下载 pkg2appimage
wget -O pkg2appimage https://github.com/AppImageCommunity/pkg2appimage/releases/download/continuous/pkg2appimage-1eceb30-x86_64.AppImage
chmod a+x pkg2appimage

# 创建临时工作目录
WORK_DIR=$(mktemp -d)
cd "$WORK_DIR"

# 设置环境变量
export VERSION="${VERSION}"
export APP_NAME="OfficeAssistant"
export OUTPUT_DIR="${OUTPUT_DIR}"

# 复制配置文件
cp "${GITHUB_WORKSPACE}/scripts/pkg2appimage.yml" ./

# 创建 AppImage
./pkg2appimage pkg2appimage.yml

# 移动生成的 AppImage 到正确的位置
find . -name "*.AppImage" -exec mv {} "${GITHUB_WORKSPACE}/${APP_NAME}-linux-x64-v${VERSION}.AppImage" \;

# 清理临时目录
cd "${GITHUB_WORKSPACE}"
rm -rf "$WORK_DIR"
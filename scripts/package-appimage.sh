#!/bin/bash

# 安装依赖
sudo apt-get update -y
sudo apt-get install -y libfuse2 desktop-file-utils

# 下载 pkg2appimage
wget -O pkg2appimage https://github.com/AppImageCommunity/pkg2appimage/releases/download/continuous/pkg2appimage-1eceb30-x86_64.AppImage
chmod a+x pkg2appimage

# 设置环境变量
export VERSION="${VERSION}"
export APP_NAME="OfficeAssistant"
export OUTPUT_DIR="${OUTPUT_DIR}"

# 创建 AppImage
./pkg2appimage ./scripts/pkg2appimage.yml
mv out/*.AppImage "${APP_NAME}-linux-x64-v${VERSION}.AppImage"
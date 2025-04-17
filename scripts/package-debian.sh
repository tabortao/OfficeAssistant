#!/bin/bash

Arch="$1"          # 架构名称
OutputPath="$2"    # 构建输出路径
Version="$3"       # 版本号

PackagePath="OfficeAssistant-Package-${Arch}"
mkdir -p "${PackagePath}/DEBIAN"
mkdir -p "${PackagePath}/opt/OfficeAssistant"

# 复制构建文件到打包目录
cp -rf $OutputPath/* "${PackagePath}/opt/OfficeAssistant"

# 设置架构标识
if [ $Arch = "linux-x64" ]; then
    Arch2="amd64" 
else
    Arch2="arm64"
fi

# 创建 control 文件
cat >"${PackagePath}/DEBIAN/control" <<-EOF
Package: officeassistant
Version: $Version
Architecture: $Arch2
Maintainer: https://github.com/TraeAI/OfficeAssistant
Description: Office Assistant - An AI-powered office productivity tool
EOF

# 创建 postinst 脚本
cat >"${PackagePath}/DEBIAN/postinst" <<-EOF
if [ ! -s /usr/share/applications/OfficeAssistant.desktop ]; then
    cat >/usr/share/applications/OfficeAssistant.desktop<<-END
[Desktop Entry]
Name=Office Assistant
Comment=AI-powered office productivity tool
Exec=/opt/OfficeAssistant/OfficeAssistant
Icon=/opt/OfficeAssistant/Resources/icon.png
Terminal=false
Type=Application
Categories=Office;Utility;
END
fi

update-desktop-database
EOF

# 设置权限
sudo chmod 0755 "${PackagePath}/DEBIAN/postinst"
sudo chmod 0755 "${PackagePath}/opt/OfficeAssistant/OfficeAssistant"

# 构建 deb 包
sudo dpkg-deb -Zxz --build $PackagePath
sudo mv "${PackagePath}.deb" "OfficeAssistant-${Arch}.deb"
#!/bin/bash

Arch="$1"          # 架构名称
OutputPath="$2"    # 构建输出路径
Version="$3"       # 版本号

# 设置变量
AppName="OfficeAssistant"
PackagePath="${AppName}-Package-${Arch}"

# 创建目录结构
mkdir -p "${PackagePath}/DEBIAN"
mkdir -p "${PackagePath}/usr/lib/${AppName}"
mkdir -p "${PackagePath}/usr/bin"
mkdir -p "${PackagePath}/usr/share/applications"

# 复制文件
cp -rf "${OutputPath}"/* "${PackagePath}/usr/lib/${AppName}/"
ln -sf "../lib/${AppName}/OfficeAssistant" "${PackagePath}/usr/bin/OfficeAssistant"

# 设置架构
if [ "$Arch" = "linux-x64" ]; then
    Arch2="amd64"
elif [ "$Arch" = "linux-arm64" ]; then
    Arch2="arm64"
fi

# 创建 control 文件
cat >"${PackagePath}/DEBIAN/control" <<EOF
Package: officeassistant
Version: ${Version}
Architecture: ${Arch2}
Maintainer: OfficeAssistant Team
Description: Office Assistant Desktop Application
Homepage: https://github.com/yourusername/OfficeAssistant
Section: utils
Priority: optional
EOF

# 创建 desktop 文件
cat >"${PackagePath}/usr/share/applications/OfficeAssistant.desktop" <<EOF
[Desktop Entry]
Name=OfficeAssistant
Comment=Office Assistant Desktop Application
Exec=/usr/bin/OfficeAssistant
Terminal=false
Type=Application
Categories=Office;Utility;
EOF

# 设置权限
chmod 755 "${PackagePath}/usr/lib/${AppName}/OfficeAssistant"
find "${PackagePath}/usr/lib/${AppName}" -type f -executable -exec chmod 755 {} \;
chmod 644 "${PackagePath}/usr/share/applications/OfficeAssistant.desktop"

# 构建 deb 包
dpkg-deb --build "${PackagePath}" "${AppName}-${Arch}-v${Version}.deb"
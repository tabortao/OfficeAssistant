#!/bin/bash

# 获取参数
Arch="$1"          # 架构名称
OutputPath="$2"    # 构建输出路径
Version="$3"       # 版本号

# 设置变量
AppName="OfficeAssistant"
OutputArch="${AppName}-${Arch}"
FileName="${AppName}-${Arch}-v${Version}.zip"

# 创建临时目录
ZipPath="./${OutputArch}"
mkdir -p "$ZipPath"

# 复制构建文件到临时目录
cp -rf "$OutputPath"/* "$ZipPath/"

# 检查并安装 zip 工具（如果在 Linux 环境）
if [ "$(uname)" = "Linux" ]; then
    if ! command -v zip &> /dev/null; then
        sudo apt-get update && sudo apt-get install -y zip
    fi
    # 使用 zip 命令创建压缩包
    zip -r "$FileName" "$ZipPath"
else
    # 在其他环境使用 7z（如果可用）
    if command -v 7z &> /dev/null; then
        7z a -tzip "$FileName" "$ZipPath"
    else
        # macOS 环境使用原生工具
        if [ "$(uname)" = "Darwin" ]; then
            # 优先使用 ditto（保留文件权限和属性）
            if command -v ditto &> /dev/null; then
                ditto -c -k --keepParent "$ZipPath" "$FileName"
            else
                # 备用方案：使用系统自带的 zip
                zip -r "$FileName" "$ZipPath"
            fi
        else
            # 非 macOS 环境使用 zip 命令
            zip -r "$FileName" "$ZipPath"
        fi
    fi
fi

# 清理临时目录
rm -rf "$ZipPath"
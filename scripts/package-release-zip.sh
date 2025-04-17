#!/bin/bash

# 获取参数
Arch="$1"          # 架构名称
OutputPath="$2"    # 构建输出路径

# 设置变量
AppName="OfficeAssistant"
OutputArch="${AppName}-${Arch}"
FileName="${AppName}-${Arch}.zip"

# 创建临时目录
ZipPath="./${OutputArch}"
mkdir -p $ZipPath

# 复制构建文件到临时目录
cp -rf $OutputPath/* "$ZipPath/"

# 创建ZIP包（使用较低压缩率以加快速度）
7z a -tZip $FileName "$ZipPath" -mx1

# 清理临时目录
rm -rf $ZipPath
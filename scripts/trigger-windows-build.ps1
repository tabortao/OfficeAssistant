# 1. 使用 winget（Windows 包管理器，Windows 10/11 自带）：
# winget install GitHub.cli
# 2. 安装完后重启系统
# 3. 安装完成后，需要登录 GitHub 账号：
# gh auth login
# 4. 仅构建（不创建发布版本）：
# .\scripts\trigger-windows-build.ps1
# 5. 构建并创建发布版本：
# .\scripts\trigger-windows-build.ps1 -ReleaseTag "v1.0.0"
param(
    [string]$ReleaseTag = ""
)

$WorkflowName = "Build Windows Desktop (Avalonia UI)"

if ($ReleaseTag -eq "") {
    Write-Host "触发构建工作流（不创建发布版本）..."
    gh workflow run $WorkflowName
}
else {
    Write-Host "触发构建工作流并创建发布版本: $ReleaseTag"
    gh workflow run $WorkflowName -f release_tag=$ReleaseTag
}

Write-Host "`n构建已触发，请在 GitHub Actions 页面查看进度。"

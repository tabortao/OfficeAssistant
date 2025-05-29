# 发布流程
使用方法：
1. VS Code 提交代码，修改好ChangeLogs.md
2. GitHub Release v*.*.*
3. GitHub Actions 自动打包应用

1. 当你要发布新版本时，只需要：
```bash
git tag v1.1.0  # 当前提交（commit）上创建一个名为 "v1.1.0" 的标签
git push origin v1.1.0 # 将刚才创建的 v1.1.0 标签推送到远程仓库（origin）
```

2. 工作流会自动：
   - 检测到 v1.1.0 标签
   - 触发 build-all.yml 进行所有平台的构建
   - 构建完成后的文件会自动上传到刚创建的 Release

注意事项：

1. 需要确保仓库设置中已启用 GitHub Actions 的写入权限
# 发布流程
使用方法：

1. 当你要发布新版本时，只需要：
```bash
git tag v1.1.0  # 当前提交（commit）上创建一个名为 "v1.1.0" 的标签
git push origin v1.1.0 # 将刚才创建的 v1.1.0 标签推送到远程仓库（origin）
```

2. 工作流会自动：
   - 检测到 v1.1.0 标签
   - 从 ChangeLogs.md 中提取对应版本的更新内容
   - 创建一个新的 GitHub Release
   - 触发 build-all.yml 进行所有平台的构建
   - 构建完成后的文件会自动上传到刚创建的 Release

注意事项：
1. 确保 ChangeLogs.md 中的版本号格式与 Git tag 完全匹配（例如：v1.1.0）
2. 版本更新内容必须按照当前的格式记录在 ChangeLogs.md 中
3. 需要确保仓库设置中已启用 GitHub Actions 的写入权限

这样就实现了完全自动化的发布流程，只需要推送一个新的版本标签就能触发整个发布流程。
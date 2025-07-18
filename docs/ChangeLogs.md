# Office Assistant 更新日志

## 环境要求
- .NET 8.0 Desktop Runtime
- 下载地址：[.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-desktop-8.0.4-windows-x64-installer)


## TODO
- 增加文件拖放功能
- 拖拽功能给出提示“可直接拖入PDF文件”

## v1.1.8（20250709）
### ✨新增功能
- PDF文件可以拖入列表中


## v1.1.7（20250530）
### ✨新增功能
- 增加PDF批量转图片功能

## v1.1.6 (20250529)
### 🔄 功能改进
- 修复已知的Bug

## v1.1.5
### 🔄 功能改进
- 美化UI，左侧增加图标导航栏
- 修复已知的Bug

## v1.1.4 
### ✨新增功能
- 增加PDF批量删除功能
- 调整项目结构，方便后续功能拓展

## v1.1.3 
### ✨新增功能
- 增加PDF批量插入功能

## v1.1.2 
### 🔄 功能改进
- 优化PDF合并、替换等功能UI

## v1.1.1
### ✨ 新增功能
- 实现PDF批量压缩功能
  - 支持多文件批量压缩
  - 提供三种预设压缩等级(低/中/高)
  - 支持自定义压缩参数(1-100)
  - 可指定输出目录

### 🔄 功能改进
- 优化压缩进度显示
- 改进压缩后文件大小估算

## v1.1.0
### 改进
- 优化版本显示
- 优化增加Github Action自动发布到Github Releases的配置文件

## v1.0.9
### 改进
- 优化UI显示
- 增加Github Action自动发布到Github Releases

## v1.0.8
### 改进
- 修改左侧导航栏UI
- 修改右侧内容区域UI

## v1.0.7
### 新增功能
- 实现PDF批量替换功能
  - 支持多文件批量替换指定页面
  - 直接覆盖源文件模式
### 改进
- 提升代码可读性
  - 增加代码注释
  - 补充完善说明文件

## v1.0.6
### 改进
- 更新项目框架至 .NET 8.0

## v1.0.5
### 修复
- 修正程序打包后图标显示问题

## v1.0.4
### 改进
- 优化导航栏UI显示效果

## v1.0.3
### 新增功能
- 添加清除所有PDF文件的功能

## v1.0.2
### 新增功能
- 实现PDF拆分功能
  - 支持单页拆分
  - 支持页码范围拆分
### 修复
- 修复PDF合并时文件顺序错乱问题

## v1.0.1
### 改进
- 精简优化项目代码结构

## v1.0.0 
### 首次发布
- 实现PDF合并基础功能
  - 支持多文件选择
  - 支持拖拽排序
  - 支持文件预览

## 版本日志编写建议

### 1. 版本号规范
- 主版本号：重大功能变更或不兼容的API修改
- 次版本号：向下兼容的功能性新增
- 修订号：向下兼容的问题修复
例如：V1.2.3

### 2. 内容分类
建议按以下类别组织：
- ✨ 新增功能 (New Features)
- 🔄 功能改进 (Improvements)
- 🐛 问题修复 (Bug Fixes)
- 🔧 维护更新 (Maintenance)
- ⚠️ 重大变更 (Breaking Changes)

### 3. 记录要点
- 使用清晰的动词开头
- 说明变更的具体影响
- 标注重要的依赖更新
- 记录不向下兼容的变更
- 添加必要的升级说明

### 4. 格式建议
```markdown
### Vx.y.z (yyyy-MM-dd)
#### ✨ 新增功能
- 添加了xxx功能
  - 支持xxx
  - 配置xxx

#### 🔄 功能改进
- 优化了xxx性能
- 改进了xxx体验

#### 🐛 问题修复
- 修复了xxx问题
```

### 5. 其他建议
- 保持时间戳的一致性
- 及时更新待办事项
- 记录所有API变更
- 标注废弃的功能
- 添加相关文档链接
> 20250417 花费一天时间，研究明白了Github Action的工作原理，可以很方便的使用github action 构建多版本应用，下面是对Github Action的工作原理的详细解释，加深下自己的理解，方便后续更好的使用。

## Github Action说明
Github Action 是一个自动化的工作流引擎，用于在 GitHub 仓库中执行各种任务。它可以与其他 GitHub 功能（如拉取请求、问题和讨论）集成，以实现持续集成、持续交付和持续部署等工作流。


### 1. 工作流文件

工作流文件是一个 YAML 文件，用于定义 GitHub Actions 的工作流程。每个工作流文件都位于仓库的 `.github/workflows` 目录下。


### 2. 工作流触发条件

工作流文件中的 `on` 字段用于定义工作流的触发条件。常见的触发条件包括：

- `push`：当有新的提交推送到仓库时触发工作流。
- `pull_request`：当有新的拉取请求创建时触发工作流。
- `schedule`：按照指定的时间表触发工作流。
- `workflow_dispatch`：通过手动触发工作流。

比如要发布应用release，在github action中，执行添加好的YAML文件，添加应用标签，就可以发布到对应的release中。
也可以自动触发，在git pull的时候，自动触发。

### 常见问题

GITHUB_TOKEN 是 GitHub Actions 自动提供的令牌，不需要手动设置。它会在工作流运行时自动创建和配置。

不过，为了确保发布权限正确，你需要在仓库的设置中检查以下内容：

1. 进入仓库设置：
   
   - 点击仓库的 "Settings" 标签页
   - 选择左侧菜单中的 "Actions" -> "General"
2. 在 "Workflow permissions" 部分：
   
   - 确保选中 "Read and write permissions"
   - 勾选 "Allow GitHub Actions to create and approve pull requests"
这样就能确保工作流有足够的权限来创建发布版本。你不需要修改工作流文件，因为 GITHUB_TOKEN 的配置已经正确。


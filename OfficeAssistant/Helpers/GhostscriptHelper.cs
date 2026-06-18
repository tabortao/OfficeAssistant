using System;
using System.IO;
using System.Runtime.InteropServices;

namespace OfficeAssistant.Helpers
{
    /// <summary>
    /// GhostScript 可执行文件路径查找工具。
    /// 优先查找应用内置的 GhostScript，其次查找系统安装的版本。
    /// </summary>
    public static class GhostscriptHelper
    {
        /// <summary>
        /// 查找 GhostScript 可执行文件的完整路径。
        /// 查找优先级：应用内 ghostscript 目录 → 应用根目录 → PATH → 常见安装目录。
        /// </summary>
        /// <returns>GhostScript 可执行文件的完整路径</returns>
        /// <exception cref="FileNotFoundException">未找到 GhostScript 时抛出</exception>
        /// <exception cref="PlatformNotSupportedException">操作系统不受支持时抛出</exception>
        public static string FindGhostscriptExecutablePath()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // 确定当前平台的 exe 名称
            string[] exeNames;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                exeNames = ["gswin64c.exe", "gswin32c.exe"];
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                     RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                exeNames = ["gs"];
            }
            else
            {
                throw new PlatformNotSupportedException("当前操作系统不受 GhostScript 支持。");
            }

            // 1. 检查应用内置 ghostscript 子目录（安装包捆绑位置）
            string bundledDir = Path.Combine(baseDir, "ghostscript");
            foreach (var exe in exeNames)
            {
                var fullPath = Path.Combine(bundledDir, exe);
                if (File.Exists(fullPath))
                    return fullPath;
            }

            // 2. 检查应用根目录（兜底）
            foreach (var exe in exeNames)
            {
                var fullPath = Path.Combine(baseDir, exe);
                if (File.Exists(fullPath))
                    return fullPath;
            }

            // 3. 遍历 PATH 环境变量
            var paths = (Environment.GetEnvironmentVariable("PATH") ?? "")
                .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

            foreach (var dir in paths)
            {
                foreach (var exe in exeNames)
                {
                    try
                    {
                        var fullPath = Path.Combine(dir.Trim(), exe);
                        if (File.Exists(fullPath))
                            return fullPath;
                    }
                    catch
                    {
                        // 忽略路径组合错误
                    }
                }
            }

            // 4. 检查常见安装目录
            string[] commonDirs;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                commonDirs =
                [
                    @"C:\Program Files\gs\gs10.05.1\bin",
                    @"D:\Program Files\gs\gs10.05.1\bin",
                    @"C:\Program Files\gs\gs10.05.0\bin",
                    @"D:\Program Files\gs\gs10.05.0\bin",
                ];
            }
            else
            {
                commonDirs =
                [
                    "/usr/bin",
                    "/usr/local/bin",
                ];
            }

            foreach (var dir in commonDirs)
            {
                foreach (var exe in exeNames)
                {
                    try
                    {
                        var fullPath = Path.Combine(dir, exe);
                        if (File.Exists(fullPath))
                            return fullPath;
                    }
                    catch
                    {
                        // 忽略路径组合错误
                    }
                }
            }

            throw new FileNotFoundException(
                "未找到 GhostScript。请确保已通过安装包安装或手动安装 GhostScript。");
        }
    }
}

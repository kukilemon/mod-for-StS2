# IreneMod

《杀戮尖塔 2》自定义角色 Mod。

## 开发环境要求

编译本项目需要：

- Windows 版《杀戮尖塔 2》
- [.NET SDK 9.0.316](https://dotnet.microsoft.com/download/dotnet/9.0)，具体版本见 `global.json`
- Godot 4.5.1 Mono，推荐使用与 StS2 对应的 MegaDot
- 完整的《杀戮尖塔 2》反编译 Godot 工程
- Git 和 PowerShell

可以使用以下命令检查 .NET SDK：

```powershell
dotnet --version
```

输出应为 `9.0.316`，或者与 `global.json` 的滚动规则兼容的更新补丁版本。

## 初始化 Godot 开发环境

本项目会复用游戏本体中的场景、脚本、图集和其他资源。仓库不会分发这些游戏资源，因此首次克隆后不能直接在 Godot/MegaDot 中完整加载项目。

请先自行准备完整的《杀戮尖塔 2》反编译 Godot 工程，然后在仓库根目录打开 PowerShell 并运行：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\initialize-dev.ps1 `
    -DecompiledProjectPath "E:\StS2_decompiled"
```

将示例路径替换为开发者自己的反编译工程路径。脚本会：

1. 检查反编译工程中是否存在必要的项目文件和本体图集。
2. 从当前 Mod 的 `res://` 引用开始，递归复制缺失的本体依赖。
3. 将复制的文件记录在 `.godot/irene_dev_dependencies.json`。
4. 将这些文件加入本仓库本地的 `.git/info/exclude`，防止误提交游戏资源。

完成后，使用 StS2 对应版本的 MegaDot 打开本仓库目录，并等待资源首次导入完成。

当游戏更新或反编译工程发生变化时，可以刷新已经补入的依赖：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\initialize-dev.ps1 `
    -DecompiledProjectPath "E:\StS2_decompiled" `
    -Refresh
```

脚本不会覆盖仓库原本存在的 Mod 文件。`-Refresh` 只覆盖此前由初始化脚本登记、且未被 Git 跟踪的本体依赖。不要将反编译产生的游戏资源提交到仓库。

## 编译 DLL

`IreneMod.csproj` 需要读取游戏安装目录中的以下程序集：

- `sts2.dll`
- `0Harmony.dll`
- `GodotSharp.dll`

这些文件通常位于：

```text
<游戏安装根目录>\data_sts2_windows_x86_64
```

在仓库根目录依次运行：

```powershell
dotnet restore .\IreneMod.csproj

dotnet build .\IreneMod.csproj `
    --configuration Release `
    -p:GameDir="D:\steam\steamapps\common\Slay the Spire 2"
```

请将 `GameDir` 替换为开发者自己的游戏安装根目录。不要把它写成 `data_sts2_windows_x86_64` 目录。

编译成功后，项目会自动把以下文件复制到 `build`：

```text
build/
├── IreneMod.dll
├── IreneMod.pdb
└── IreneMod.json
```

其中 `IreneMod.pdb` 只用于调试。发布时必须包含 DLL 和 manifest JSON。

如果 Rider 已经正确使用仓库中的 .NET SDK，也可以打开 `IreneMod.csproj` 后选择 `Release` 配置进行构建。首次构建前仍需确保 `GameDir` 指向正确的游戏安装位置；命令行参数不会永久修改项目文件。

## 打包 PCK

DLL 编译完成且 MegaDot 已完成资源导入后，运行：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\pack-pck.ps1 `
    -MegaDotPath "D:\megadot_4.5.1\MegaDot_v4.5.1-stable_mono_win64_console.exe"
```

将 `MegaDotPath` 替换为开发者自己的 MegaDot 控制台程序路径。打包成功后会生成：

```text
build/IreneMod.pck
```

最终 `build` 目录中用于加载 Mod 的主要文件为：

```text
IreneMod.dll
IreneMod.json
IreneMod.pck
```

推荐的完整构建顺序是：

1. 克隆仓库。
2. 运行 `initialize-dev.ps1` 补齐本体开发依赖。
3. 使用 MegaDot 打开项目并等待资源导入完成。
4. 使用 `dotnet build` 编译 DLL。
5. 使用 `pack-pck.ps1` 生成 PCK。

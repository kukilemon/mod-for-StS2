# EireneMod

《杀戮尖塔 2》自定义角色 Mod。

## 初始化 Godot 开发环境

本项目会复用游戏本体中的场景、脚本、图集和其他资源。仓库不会分发这些游戏资源，因此首次克隆后不能直接在 Godot/MegaDot 中完整加载项目。

请先自行准备完整的《杀戮尖塔 2》反编译 Godot 工程，然后在 PowerShell 中运行：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\initialize-dev.ps1 `
    -DecompiledProjectPath "E:\StS2_decompiled"
```

脚本会：

1. 检查反编译工程中是否存在必要的项目文件和本体图集。
2. 从当前 Mod 的 `res://` 引用开始，递归复制缺失的本体依赖。
3. 将复制的文件记录在 `.godot/eirene_dev_dependencies.json`。
4. 将这些文件加入本仓库本地的 `.git/info/exclude`，防止误提交游戏资源。

完成后，使用 StS2 对应版本的 MegaDot 打开本仓库目录。

当游戏更新或反编译工程发生变化时，可以刷新已经补入的依赖：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\initialize-dev.ps1 `
    -DecompiledProjectPath "E:\StS2_decompiled" `
    -Refresh
```

脚本不会覆盖仓库原本存在的 Mod 文件。`-Refresh` 只覆盖此前由初始化脚本登记的本体依赖。

如果反编译工程不在上述示例路径，只需将参数替换成开发者自己的路径。不要将反编译产生的游戏资源提交到仓库。

## 打包 PCK

```powershell
.\tools\pack-pck.ps1 -MegaDotPath "D:\megadot_4.5.1\MegaDot_v4.5.1-stable_mono_win64_console.exe"
```

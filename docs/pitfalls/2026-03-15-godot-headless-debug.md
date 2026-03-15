# Godot 项目主动调试方法

- **日期**: 2026-03-15
- **关键词**: Transform3D, .csproj, headless, 摄像机朝向
- **影响范围**: scenes/, scripts/

## 现象
手写 .tscn 中的 Transform3D 导致摄像机朝向错误，但无法直观判断问题所在。用户反复测试仍看不到场景内容。

## 根因
1. 手写 Transform3D 旋转矩阵容易搞错符号（Godot .tscn 格式按行存储列向量分量）
2. 缺少 .csproj 导致 C# 脚本不被编译，运行时静默失败
3. 没有自动化手段验证场景配置是否正确

## 解决方案
建立**命令行自动化调试流程**：

1. **在关键节点的 `_Ready()` 中输出调试信息**：
   ```csharp
   var t = cam.GlobalTransform;
   GD.Print($"[Camera] 位置={t.Origin}");
   GD.Print($"[Camera] 朝向={-t.Basis.Z}");
   ```

2. **通过命令行运行 Godot 捕获输出**：
   ```powershell
   $godot = "D:\Godot\Godot_v4.6.1-stable_mono_win64_console.exe"
   $proc = Start-Process -FilePath $godot `
     -ArgumentList "--path", "`"项目路径`"", "--quit-after", "3" `
     -NoNewWindow -PassThru `
     -RedirectStandardOutput "stdout.log" `
     -RedirectStandardError "stderr.log"
   $proc.WaitForExit(20000)
   ```

3. **构建前确保 .csproj 存在**：
   ```powershell
   dotnet build "Coin Flipper.csproj"
   ```

## 教训
- 涉及 3D 变换时，不要盲目手算 Transform3D，优先用调试输出验证
- Godot C# 项目必须有 .csproj 才能编译，`--build-solutions` 在 headless 模式下不会自动生成
- 建立"写代码 → 构建 → 命令行运行 → 捕获日志 → 验证"的自动化闭环，减少对用户手动测试的依赖

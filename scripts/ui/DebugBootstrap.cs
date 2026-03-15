using Godot;

/// <summary>
/// 调试功能引导器。集中管理所有调试相关的初始化逻辑，
/// 与 GameManager 的业务职责分离。
/// 由 GameManager 在 EnableDebug=true 时调用。
/// </summary>
public static class DebugBootstrap
{
    public static void Setup(Node root)
    {
        PrintSceneDebugInfo(root);

        // 调试 UI 面板（纯显示）
        var debugUI = new DebugUI();
        debugUI.Name = "DebugUI";

        // 调试快捷键处理器（输入）
        var debugInput = new DebugInputHandler();
        debugInput.Name = "DebugInputHandler";

        var canvas = new CanvasLayer();
        canvas.Name = "DebugCanvas";
        canvas.Layer = 100;
        canvas.AddChild(debugUI);
        canvas.AddChild(debugInput);
        root.AddChild(canvas);

        GD.Print("调试面板已加载");
        GD.Print("快捷键: +/- 调力度, [/] 调精准度");
    }

    private static void PrintSceneDebugInfo(Node root)
    {
        var cam = root.GetViewport().GetCamera3D();
        if (cam != null)
        {
            var t = cam.GlobalTransform;
            var lookDir = -t.Basis.Z;
            var pos = t.Origin;
            GD.Print($"[Camera] 位置=({pos.X:F2}, {pos.Y:F2}, {pos.Z:F2})");
            GD.Print($"[Camera] 朝向=({lookDir.X:F2}, {lookDir.Y:F2}, {lookDir.Z:F2})");
            GD.Print($"[Camera] FOV={cam.Fov}");
        }
        else
        {
            GD.PrintErr("[Camera] 未找到摄像机!");
        }

        var table = root.FindChild(SceneNodes.Table, true, false)
            as Node3D;
        if (table != null)
        {
            var p = table.GlobalPosition;
            GD.Print($"[Table] 位置=({p.X:F2}, {p.Y:F2}, {p.Z:F2})");
        }
        else
        {
            GD.PrintErr($"[Table] 未找到节点 '{SceneNodes.Table}'");
        }

        var container = root.FindChild(
            SceneNodes.CoinContainer, true, false);
        if (container != null)
        {
            int count = 0;
            foreach (var child in container.GetChildren())
            {
                if (child is Node3D n3d)
                {
                    var p = n3d.GlobalPosition;
                    GD.Print($"[Coin] {n3d.Name} 位置=({p.X:F2}, {p.Y:F2}, {p.Z:F2})");
                    count++;
                }
            }
            GD.Print($"[Coins] 共 {count} 个硬币");
        }
        else
        {
            GD.PrintErr($"[CoinContainer] 未找到节点 '{SceneNodes.CoinContainer}'");
        }

        var vp = root.GetViewport();
        GD.Print($"[Viewport] 大小={vp.GetVisibleRect().Size}");
    }
}

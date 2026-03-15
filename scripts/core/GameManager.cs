using Godot;
using System.Linq;

/// <summary>
/// 游戏主场景管理器。
/// 负责初始化场景、管理全局游戏状态。
/// 启动时输出详细的调试信息用于验证场景配置。
/// </summary>
public partial class GameManager : Node3D
{
    public override void _Ready()
    {
        GD.Print("=== CoinFlipper 启动 ===");
        PrintSceneDebugInfo();

        // 动态添加调试 UI
        var debugUI = new DebugUI();
        debugUI.Name = "DebugUI";
        var canvas = new CanvasLayer();
        canvas.Name = "DebugCanvas";
        canvas.Layer = 100;
        canvas.AddChild(debugUI);
        AddChild(canvas);

        GD.Print("调试面板已加载");
        GD.Print("快捷键: +/- 调力度, [/] 调精准度");
    }

    /// <summary>输出场景中所有关键节点的位置和朝向信息</summary>
    private void PrintSceneDebugInfo()
    {
        // 摄像机信息
        var cam = GetViewport().GetCamera3D();
        if (cam != null)
        {
            var t = cam.GlobalTransform;
            var lookDir = -t.Basis.Z;
            var pos = t.Origin;
            GD.Print($"[Camera] 位置=({pos.X:F2}, {pos.Y:F2}, {pos.Z:F2})");
            GD.Print($"[Camera] 朝向=({lookDir.X:F2}, {lookDir.Y:F2}, {lookDir.Z:F2})");
            GD.Print($"[Camera] Up=({t.Basis.Y.X:F2}, {t.Basis.Y.Y:F2}, {t.Basis.Y.Z:F2})");
            GD.Print($"[Camera] FOV={cam.Fov}");
        }
        else
        {
            GD.PrintErr("[Camera] 未找到摄像机!");
        }

        // 桌面信息
        var table = FindChild("Table", true, false) as Node3D;
        if (table != null)
        {
            GD.Print($"[Table] 位置=({table.GlobalPosition.X:F2}, {table.GlobalPosition.Y:F2}, {table.GlobalPosition.Z:F2})");
        }

        // 硬币信息
        var container = FindChild("CoinContainer", true, false);
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

        // 视口信息
        var vp = GetViewport();
        GD.Print($"[Viewport] 大小={vp.GetVisibleRect().Size}");
    }
}

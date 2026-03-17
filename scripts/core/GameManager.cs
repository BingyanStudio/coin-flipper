using Godot;

/// <summary>
/// 游戏主场景管理器。
/// 负责初始化场景、管理全局游戏状态。
/// </summary>
public partial class GameManager : Node3D
{
    /// <summary>是否启用调试模式（启动日志 + 调试面板）</summary>
    [Export] public bool EnableDebug { get; set; } = true;

    public override void _Ready()
    {
        GD.Print("=== CoinFlipper 启动 ===");

        // 初始化全局服务（必须在其他系统之前）
        GameServices.Initialize();
        GD.Print("[GameManager] 全局服务已初始化");

        if (EnableDebug)
        {
            DebugBootstrap.Setup(this);
        }
    }

    public override void _ExitTree()
    {
        GameServices.Cleanup();
    }
}

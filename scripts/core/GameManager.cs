using Godot;

/// <summary>
/// 游戏主场景管理器。
/// 负责初始化场景、管理全局游戏状态。
/// 服务于 Milestone 0-1：项目骨架 + 核心物理原型。
/// </summary>
public partial class GameManager : Node3D
{
    public override void _Ready()
    {
        GD.Print("CoinFlipper: GameManager 已初始化");

        // 动态添加调试 UI
        var debugUI = new DebugUI();
        debugUI.Name = "DebugUI";
        // 使用 CanvasLayer 确保 UI 在最上层
        var canvas = new CanvasLayer();
        canvas.Name = "DebugCanvas";
        canvas.Layer = 100;
        canvas.AddChild(debugUI);
        AddChild(canvas);

        GD.Print("调试面板已加载");
        GD.Print("快捷键: +/- 调力度, [/] 调精准度");
    }
}

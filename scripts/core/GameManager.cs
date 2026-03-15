using Godot;

/// <summary>
/// 游戏主场景管理器。
/// 负责初始化场景、管理摄像机和全局游戏状态。
/// 服务于 Milestone 0：项目骨架搭建。
/// </summary>
public partial class GameManager : Node3D
{
    public override void _Ready()
    {
        GD.Print("CoinFlipper: GameManager 已初始化");
    }
}

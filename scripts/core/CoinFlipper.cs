using Godot;

/// <summary>
/// 硬币翻转交互控制器。处理玩家点击输入和射线检测。
/// 服务于 Milestone 1：核心物理原型。
/// 
/// 职责：
/// - 鼠标点击射线检测
/// - 判断点击位置相对于硬币中心的偏移
/// - 计算力度（基础力度 ± 随机浮动）
/// - 调用 Coin.ApplyFlip()
/// </summary>
public partial class CoinFlipper : Node3D
{
    /// <summary>基础翻转力度（下限由 MinBaseForce 约束）</summary>
    private float _baseForce = 2f;
    [Export] public float BaseForce
    {
        get => _baseForce;
        set => _baseForce = Mathf.Max(value, MinBaseForce);
    }

    /// <summary>力度随机浮动范围（±百分比）</summary>
    [Export] public float ForceVariance { get; set; } = 0.3f;

    /// <summary>力度下限，防止力度被调到无效值</summary>
    [Export] public float MinBaseForce { get; set; } = 1f;

    /// <summary>精准属性（0-1），越高浮动越小</summary>
    [Export] public float Precision { get; set; } = 0.5f;

    /// <summary>射线检测长度</summary>
    [Export] public float RayLength { get; set; } = 100f;

    private Camera3D _camera;

    public override void _Ready()
    {
        _camera = GetViewport().GetCamera3D();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb
            && mb.ButtonIndex == MouseButton.Left
            && mb.Pressed)
        {
            TryFlipCoin(mb.Position);
        }
    }

    /// <summary>尝试翻转鼠标位置下的硬币</summary>
    private void TryFlipCoin(Vector2 screenPos)
    {
        // 从摄像机发射射线
        Vector3 from = _camera.ProjectRayOrigin(screenPos);
        Vector3 dir = _camera.ProjectRayNormal(screenPos);
        Vector3 to = from + dir * RayLength;

        var spaceState = GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(from, to);
        var result = spaceState.IntersectRay(query);

        if (result.Count == 0) return;

        // 检查是否点击到了硬币
        var collider = result["collider"].AsGodotObject();
        if (collider is not Coin coin) return;

        // 计算力度：基础力度 ± 浮动
        float actualVariance = ForceVariance * (1f - Precision);
        float rand = (float)GD.RandRange(-actualVariance, actualVariance);
        float force = BaseForce * (1f + rand);

        // 获取点击的世界坐标
        Vector3 hitPoint = result["position"].AsVector3();

        coin.ApplyFlip(hitPoint, force);
    }
}

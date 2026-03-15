using Godot;

/// <summary>
/// 硬币翻转交互控制器。处理玩家点击输入和射线检测。
/// 服务于 Milestone 1：核心物理原型。
/// 
/// 职责：
/// - 鼠标点击射线检测
/// - 追踪鼠标移动速度
/// - 判断点击位置相对于硬币中心的偏移
/// - 计算力度（基础力度 ± 随机浮动）
/// - 将鼠标速度转换为水平方向力分量
/// - 调用 Coin.ApplyFlip()
/// </summary>
public partial class CoinFlipper : Node3D
{
    /// <summary>基础翻转力度</summary>
    [Export] public float BaseForce { get; set; } = 2f;

    /// <summary>力度随机浮动范围（±百分比）</summary>
    [Export] public float ForceVariance { get; set; } = 0.3f;

    /// <summary>精准属性（0-1），越高浮动越小</summary>
    [Export] public float Precision { get; set; } = 0.5f;

    /// <summary>鼠标速度对翻转方向的影响系数</summary>
    [Export] public float MouseVelocityInfluence { get; set; } = 0.0003f;

    /// <summary>射线检测长度</summary>
    [Export] public float RayLength { get; set; } = 100f;

    private Camera3D _camera;
    private Vector2 _mouseVelocity;
    private Vector2 _lastMousePos;
    private bool _hasLastPos;

    public override void _Ready()
    {
        _camera = GetViewport().GetCamera3D();
    }

    public override void _Input(InputEvent @event)
    {
        // 追踪鼠标移动速度
        if (@event is InputEventMouseMotion motion)
        {
            _mouseVelocity = motion.Velocity;
        }
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
    /// <summary>尝试翻转鼠标位置下的硬币</summary>
    private void TryFlipCoin(Vector2 screenPos)
    {
        Vector3 from = _camera.ProjectRayOrigin(screenPos);
        Vector3 dir = _camera.ProjectRayNormal(screenPos);
        Vector3 to = from + dir * RayLength;

        var spaceState = GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(from, to);
        var result = spaceState.IntersectRay(query);

        if (result.Count == 0) return;

        var collider = result["collider"].AsGodotObject();
        if (collider is not Coin coin) return;

        // 计算力度
        float actualVariance = ForceVariance * (1f - Precision);
        float rand = (float)GD.RandRange(
            -actualVariance, actualVariance);
        float force = BaseForce * (1f + rand);

        // 获取点击的世界坐标
        Vector3 hitPoint = result["position"].AsVector3();

        // 将屏幕空间鼠标速度转换为世界空间水平方向
        Vector3 mouseWorldDir = Vector3.Zero;
        if (_mouseVelocity.LengthSquared() > 0.1f)
        {
            mouseWorldDir = new Vector3(
                _mouseVelocity.X, 0, _mouseVelocity.Y
            ) * MouseVelocityInfluence;
        }

        coin.ApplyFlip(hitPoint, force, mouseWorldDir);
        GD.Print($"翻转! 力度={force:F2} 鼠标速度=({_mouseVelocity.X:F0},{_mouseVelocity.Y:F0}) px/s");

        // 重置鼠标速度
        _mouseVelocity = Vector2.Zero;
    }
}

using Godot;

/// <summary>
/// 硬币基础类。管理单个硬币的物理行为和状态判定。
/// 服务于 Milestone 1：核心物理原型。
/// 
/// 物理参数分工：
/// - Godot 原生属性（mass, linear_damp, angular_damp, friction, bounce）在 Coin.tscn 中配置
/// - 自定义逻辑参数（阻尼系数、检测阈值等）通过下方 [Export] 属性配置
/// 
/// 职责：
/// - 物理翻转（分离平移冲量和旋转冲量）
/// - 静止检测
/// - 正反面判定
/// - 立起来判定
/// - 翻转次数计数（累计旋转角度 / 360°）
/// - 各向异性角阻尼（模拟真实进动效果）
/// </summary>
public partial class Coin : RigidBody3D
{
    /// <summary>硬币面值</summary>
    [Export] public int FaceValue { get; set; } = 1;

    /// <summary>旋转冲量倍率，控制翻转速度</summary>
    [Export] public float TorqueMultiplier { get; set; } = 3f;

    /// <summary>硬币是否已静止</summary>
    public bool IsSettled { get; private set; }

    /// <summary>正面朝上为 true</summary>
    public bool IsFaceUp { get; private set; }

    /// <summary>是否立起来</summary>
    public bool IsStanding { get; private set; }

    /// <summary>本次弹起的物理翻转次数</summary>
    public int FlipCount { get; private set; }

    /// <summary>静止检测的线速度阈值，低于此值视为接近静止</summary>
    [Export] public float SettleLinearThreshold { get; set; } = 0.05f;

    /// <summary>静止检测的角速度阈值，低于此值视为接近静止</summary>
    [Export] public float SettleAngularThreshold { get; set; } = 0.1f;

    /// <summary>判定为静止所需的持续时间（秒）</summary>
    [Export] public float SettleTimeRequired { get; set; } = 0.5f;

    /// <summary>判定为"立起来"的角度阈值（度），与垂直方向夹角大于此值视为立起</summary>
    [Export] public float StandingAngleThreshold { get; set; } = 70f;

    /// <summary>轴向阻尼系数，绕硬币法线方向(Y轴)的旋转衰减率，值越小进动持续越久</summary>
    [Export] public float AxialDamp { get; set; } = 0.1f;

    /// <summary>径向阻尼系数，绕硬币径向(X/Z轴)的旋转衰减率，值越大翻转衰减越快</summary>
    [Export] public float RadialDamp { get; set; } = 0.8f;

    /// <summary>点击硬币中心时的扭矩比例，用于随机旋转强度</summary>
    [Export] public float CenterClickTorqueRatio { get; set; } = 0.5f;

    /// <summary>翻转力度的最小值，防止力度过小导致硬币几乎不动</summary>
    [Export] public float MinFlipForce { get; set; } = 0.5f;

    // 内部状态
    private float _settleTimer;
    private bool _isAirborne;
    private float _cumulativeRotation;
    private Vector3 _previousUp;

    public override void _Ready()
    {
        _previousUp = GlobalTransform.Basis.Y;
    }

    public override void _PhysicsProcess(double delta)
    {
        UpdateSettleDetection((float)delta);
        UpdateFlipTracking();
        if (IsSettled)
        {
            UpdateFaceDetection();
            UpdateStandingDetection();
        }
    }

    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    {
        // 各向异性角阻尼：模拟真实硬币的进动效果
        // 绕硬币自身 Y 轴（法线方向）的旋转衰减慢
        // 绕硬币自身 X/Z 轴（径向）的旋转衰减快
        if (!_isAirborne) return;

        Vector3 localAngVel = GlobalTransform.Basis.Inverse() *
            state.AngularVelocity;

        Vector3 dampedLocal = new Vector3(
            localAngVel.X * (1f - RadialDamp * (float)state.Step),
            localAngVel.Y * (1f - AxialDamp * (float)state.Step),
            localAngVel.Z * (1f - RadialDamp * (float)state.Step)
        );

        state.AngularVelocity = GlobalTransform.Basis * dampedLocal;
    }

    /// <summary>
    /// 对硬币施加翻转力。由 CoinFlipper 调用。
    /// 分离为平移冲量（控制高度）和旋转冲量（控制翻转）。
    /// </summary>
    public void ApplyFlip(Vector3 hitPoint, float force)
    {
        // 重置状态
        IsSettled = false;
        _settleTimer = 0f;
        _isAirborne = true;
        _cumulativeRotation = 0f;
        FlipCount = 0;
        _previousUp = GlobalTransform.Basis.Y;

        // 力度下限保护
        force = Mathf.Max(force, MinFlipForce);

        // 1. 平移冲量：纯向上，控制弹起高度
        ApplyCentralImpulse(Vector3.Up * force);

        // 2. 旋转冲量：基于点击偏移方向
        Vector3 offset = hitPoint - GlobalPosition;
        Vector3 horizontalOffset = new Vector3(
            offset.X, 0, offset.Z);

        if (horizontalOffset.LengthSquared() > 0.001f)
        {
            Vector3 torqueAxis = horizontalOffset.Cross(Vector3.Up)
                .Normalized();
            float torqueMag = force * TorqueMultiplier
                * horizontalOffset.Length();
            ApplyTorqueImpulse(torqueAxis * torqueMag);
        }
        else
        {
            Vector3 randomAxis = new Vector3(
                (float)GD.RandRange(-1, 1), 0,
                (float)GD.RandRange(-1, 1)).Normalized();
            ApplyTorqueImpulse(randomAxis * force * CenterClickTorqueRatio);
        }
    }

    private void UpdateSettleDetection(float delta)
    {
        if (IsSettled) return;

        bool isSlow =
            LinearVelocity.Length() < SettleLinearThreshold
            && AngularVelocity.Length() < SettleAngularThreshold;

        if (isSlow)
        {
            _settleTimer += delta;
            if (_settleTimer >= SettleTimeRequired)
            {
                IsSettled = true;
                _isAirborne = false;
            }
        }
        else
        {
            _settleTimer = 0f;
        }
    }

    private void UpdateFaceDetection()
    {
        float dot = GlobalTransform.Basis.Y.Dot(Vector3.Up);
        IsFaceUp = dot > 0;
    }

    private void UpdateStandingDetection()
    {
        float dot = Mathf.Abs(
            GlobalTransform.Basis.Y.Dot(Vector3.Up));
        float angleDeg = Mathf.RadToDeg(Mathf.Acos(dot));
        IsStanding = angleDeg > StandingAngleThreshold;
    }

    private void UpdateFlipTracking()
    {
        if (!_isAirborne) return;

        Vector3 currentUp = GlobalTransform.Basis.Y;
        float dot = Mathf.Clamp(
            _previousUp.Dot(currentUp), -1f, 1f);
        float angleDelta = Mathf.RadToDeg(Mathf.Acos(dot));
        _cumulativeRotation += angleDelta;
        FlipCount = (int)(_cumulativeRotation / 360f);
        _previousUp = currentUp;
    }
}

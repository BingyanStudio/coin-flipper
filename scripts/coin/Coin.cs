using Godot;

/// <summary>
/// 硬币基础类。管理单个硬币的物理行为和状态判定。
/// 服务于 Milestone 1：核心物理原型。
/// 
/// 职责：
/// - 物理翻转（接收力和力矩）
/// - 静止检测
/// - 正反面判定
/// - 立起来判定
/// - 翻转次数计数（累计旋转角度 / 360°）
/// </summary>
public partial class Coin : RigidBody3D
{
    /// <summary>硬币面值</summary>
    [Export] public int FaceValue { get; set; } = 1;

    /// <summary>硬币是否已静止</summary>
    public bool IsSettled { get; private set; }

    /// <summary>正面朝上为 true</summary>
    public bool IsFaceUp { get; private set; }

    /// <summary>是否立起来</summary>
    public bool IsStanding { get; private set; }

    /// <summary>本次弹起的物理翻转次数</summary>
    public int FlipCount { get; private set; }

    // 静止检测阈值
    private const float SettleLinearThreshold = 0.05f;
    private const float SettleAngularThreshold = 0.1f;
    private const float SettleTimeRequired = 0.5f;
    // 立起来判定：up 向量与世界 up 夹角 > 70° 视为立起来
    private const float StandingAngleThreshold = 70f;

    // 内部状态
    private float _settleTimer;
    private bool _isAirborne;
    private float _cumulativeRotation;
    private Vector3 _previousUp;

    public override void _Ready()
    {
        _previousUp = GlobalTransform.Basis.Y;
        ContactMonitor = true;
        MaxContactsReported = 4;
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

    /// <summary>
    /// 对硬币施加翻转力。由 CoinFlipper 调用。
    /// </summary>
    /// <param name="hitPoint">世界空间中的点击位置</param>
    /// <param name="force">施加的力大小</param>
    public void ApplyFlip(Vector3 hitPoint, float force)
    {
        // 重置状态
        IsSettled = false;
        _settleTimer = 0f;
        _isAirborne = true;
        _cumulativeRotation = 0f;
        FlipCount = 0;
        _previousUp = GlobalTransform.Basis.Y;

        // 计算点击偏移：从硬币中心到点击点
        Vector3 offset = hitPoint - GlobalPosition;
        // 在偏移位置施加向上冲量，自然产生力矩（无需额外 TorqueImpulse）
        ApplyImpulse(Vector3.Up * force, offset);
    }

    /// <summary>静止检测：线速度和角速度均低于阈值持续一段时间</summary>
    private void UpdateSettleDetection(float delta)
    {
        if (IsSettled) return;

        bool isSlow = LinearVelocity.Length() < SettleLinearThreshold
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

    /// <summary>正反面判定：up 向量与世界 up 的点积</summary>
    private void UpdateFaceDetection()
    {
        float dot = GlobalTransform.Basis.Y.Dot(Vector3.Up);
        IsFaceUp = dot > 0;
    }

    /// <summary>立起来判定：up 向量与世界 up 的夹角</summary>
    private void UpdateStandingDetection()
    {
        float dot = Mathf.Abs(GlobalTransform.Basis.Y.Dot(Vector3.Up));
        float angleDeg = Mathf.RadToDeg(Mathf.Acos(dot));
        IsStanding = angleDeg > StandingAngleThreshold;
    }

    /// <summary>翻转次数追踪：累计旋转角度 / 360°</summary>
    private void UpdateFlipTracking()
    {
        if (!_isAirborne) return;

        Vector3 currentUp = GlobalTransform.Basis.Y;
        float dot = Mathf.Clamp(_previousUp.Dot(currentUp), -1f, 1f);
        float angleDelta = Mathf.RadToDeg(Mathf.Acos(dot));
        _cumulativeRotation += angleDelta;
        FlipCount = (int)(_cumulativeRotation / 360f);
        _previousUp = currentUp;
    }
}

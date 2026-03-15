/// <summary>
/// 标签触发时机枚举。定义标签可以响应的游戏事件类型。
/// 一个标签可以响应多个时机（通过 HandleEvent 内部判断）。
/// </summary>
public enum TagTrigger
{
    /// <summary>硬币被翻转时</summary>
    OnFlip,

    /// <summary>硬币落地静止时</summary>
    OnLand,

    /// <summary>结算阶段计分时</summary>
    OnScore,

    /// <summary>被其他硬币砸中时</summary>
    OnHit,

    /// <summary>掉出场地时</summary>
    OnFallOff,

    /// <summary>持续性效果（被动属性修饰）</summary>
    Passive,
}

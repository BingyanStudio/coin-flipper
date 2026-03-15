/// <summary>
/// 游戏事件定义。所有标签系统使用的事件类型。
/// 事件是纯数据对象，描述"发生了什么"，不包含行为逻辑。
/// 服务于 Milestone 2：标签系统核心。
/// </summary>

/// <summary>所有游戏事件的基类</summary>
public abstract class GameEvent
{
    /// <summary>事件发生时的深度（用于防止无限循环）</summary>
    public int Depth { get; set; }
}

/// <summary>硬币被翻转时触发（ApplyFlip 调用后）</summary>
public class CoinFlippedEvent : GameEvent
{
    /// <summary>被翻转的硬币</summary>
    public Coin Coin { get; init; }

    /// <summary>施加的力度</summary>
    public float Force { get; init; }

    /// <summary>点击位置（世界坐标）</summary>
    public Godot.Vector3 HitPoint { get; init; }
}

/// <summary>硬币静止后触发</summary>
public class CoinSettledEvent : GameEvent
{
    public Coin Coin { get; init; }

    /// <summary>是否正面朝上</summary>
    public bool IsFaceUp { get; init; }

    /// <summary>是否立起来</summary>
    public bool IsStanding { get; init; }

    /// <summary>本次翻转次数</summary>
    public int FlipCount { get; init; }
}

/// <summary>硬币被其他硬币砸中时触发</summary>
public class CoinHitEvent : GameEvent
{
    /// <summary>被砸的硬币</summary>
    public Coin Target { get; init; }

    /// <summary>砸过来的硬币</summary>
    public Coin Hitter { get; init; }

    /// <summary>碰撞力度</summary>
    public float ImpactForce { get; init; }
}

/// <summary>硬币掉出场地时触发</summary>
public class CoinFellOffEvent : GameEvent
{
    public Coin Coin { get; init; }

    /// <summary>掉落前的位置</summary>
    public Godot.Vector3 LastPosition { get; init; }
}

/// <summary>结算阶段开始时触发（对每个硬币）</summary>
public class ScorePhaseEvent : GameEvent
{
    public Coin Coin { get; init; }
}

/// <summary>回合结束时触发</summary>
public class RoundEndEvent : GameEvent { }

/// <summary>新硬币生成时触发</summary>
public class CoinSpawnedEvent : GameEvent
{
    public Coin Coin { get; init; }
}

/// <summary>硬币被移除时触发</summary>
public class CoinRemovedEvent : GameEvent
{
    public Coin Coin { get; init; }
}

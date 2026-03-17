using System;
using System.Linq;
using Godot;

/// <summary>
/// 【磁力】标签：落地后吸引周围硬币。
/// 响应 CoinSettledEvent，在硬币静止后对周围硬币施加吸引力。
/// 吸引力在一段时间内持续作用，然后停止。
/// 
/// 触发时机：OnLand
/// 效果：硬币落地后，吸引一定范围内的其他硬币向自身靠拢
/// </summary>
public class MagneticTag : ITagBehavior
{
    public string TagId => TagIds.Magnetic;

    /// <summary>磁力吸引半径</summary>
    private const float AttractRadius = 3f;

    /// <summary>磁力吸引力度</summary>
    private const float AttractForce = 0.8f;

    /// <summary>磁力持续时间（秒）</summary>
    private const float AttractDuration = 1.0f;

    private Action _unsubFlip;
    private Action _unsubSettle;
    private Coin _coin;
    private bool _canAttract;

    public void OnAttached(TagContext context)
    {
        _coin = context.Coin;
        _unsubFlip = context.EventBus.Subscribe<CoinFlippedEvent>(OnFlipped);
        _unsubSettle = context.EventBus.Subscribe<CoinSettledEvent>(OnSettled);
    }

    public void OnDetached(TagContext context)
    {
        _unsubFlip?.Invoke();
        _unsubSettle?.Invoke();
        _unsubFlip = null;
        _unsubSettle = null;
    }

    private void OnFlipped(CoinFlippedEvent evt)
    {
        if (evt.Coin != _coin) return;
        _canAttract = true;
    }

    private void OnSettled(CoinSettledEvent evt)
    {
        if (evt.Coin != _coin || !_canAttract) return;
        _canAttract = false;

        GD.Print($"[MagneticTag] {_coin.Name}: 磁力吸引启动！");

        // 获取 CoinContainer 中的所有硬币
        var container = _coin.GetParent();
        if (container == null) return;

        foreach (var child in container.GetChildren())
        {
            if (child is not Coin other || other == _coin) continue;

            Vector3 direction = _coin.GlobalPosition - other.GlobalPosition;
            float distance = direction.Length();

            if (distance > AttractRadius || distance < 0.01f) continue;

            // 力度随距离衰减
            float strength = AttractForce * (1f - distance / AttractRadius);
            Vector3 impulse = direction.Normalized() * strength;

            other.ApplyCentralImpulse(impulse);
            GD.Print($"[MagneticTag] 吸引 {other.Name}，距离={distance:F2}，力度={strength:F2}");
        }
    }
}

using System;
using Godot;

/// <summary>
/// 【再弹跳】标签：落地后再弹跳一次。
/// 响应 CoinSettledEvent，在硬币首次静止时施加一个向上的冲量。
/// 使用标记位确保每次翻转只触发一次再弹跳。
/// 
/// 触发时机：OnLand
/// 效果：硬币落地静止后，自动再弹起一次
/// </summary>
public class BounceTag : ITagBehavior
{
    public string TagId => TagIds.Bounce;

    /// <summary>再弹跳的力度倍率（相对于原始翻转力度）</summary>
    private const float BounceForceRatio = 0.5f;

    /// <summary>再弹跳的基础力度</summary>
    private const float BounceBaseForce = 1.5f;

    private Action _unsubFlip;
    private Action _unsubSettle;
    private Coin _coin;
    private bool _canBounce;

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
        _canBounce = true;
    }

    private void OnSettled(CoinSettledEvent evt)
    {
        if (evt.Coin != _coin || !_canBounce) return;
        _canBounce = false;

        GD.Print($"[BounceTag] {_coin.Name}: 再弹跳！");

        // 施加向上冲量 + 轻微随机旋转
        _coin.ApplyCentralImpulse(Vector3.Up * BounceBaseForce);

        Vector3 randomTorque = new Vector3(
            (float)GD.RandRange(-0.5, 0.5),
            0,
            (float)GD.RandRange(-0.5, 0.5)
        );
        _coin.ApplyTorqueImpulse(randomTorque);
    }
}

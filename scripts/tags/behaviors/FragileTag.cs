using System;
using Godot;

/// <summary>
/// 【脆弱】标签：翻转后硬币消失。
/// 响应 CoinFlippedEvent，当宿主硬币被翻转时标记移除。
/// 实际移除在硬币落地静止后执行，避免翻转中途消失的视觉突兀。
/// 
/// 触发时机：OnFlip → OnLand（延迟执行）
/// 效果：硬币被翻转后，落地即消失
/// </summary>
public class FragileTag : ITagBehavior
{
    public string TagId => TagIds.Fragile;

    private Action _unsubFlip;
    private Action _unsubSettle;
    private Coin _coin;
    private bool _pendingRemoval;

    public void OnAttached(TagContext context)
    {
        _coin = context.Coin;
        _unsubFlip = context.EventBus.Subscribe<CoinFlippedEvent>(OnCoinFlipped);
        _unsubSettle = context.EventBus.Subscribe<CoinSettledEvent>(OnCoinSettled);
    }

    public void OnDetached(TagContext context)
    {
        _unsubFlip?.Invoke();
        _unsubSettle?.Invoke();
        _unsubFlip = null;
        _unsubSettle = null;
    }

    private void OnCoinFlipped(CoinFlippedEvent evt)
    {
        if (evt.Coin != _coin) return;
        _pendingRemoval = true;
        GD.Print($"[FragileTag] {_coin.Name}: 脆弱硬币被翻转，等待落地后移除");
    }

    private void OnCoinSettled(CoinSettledEvent evt)
    {
        if (evt.Coin != _coin || !_pendingRemoval) return;
        _pendingRemoval = false;
        GD.Print($"[FragileTag] {_coin.Name}: 脆弱硬币落地，移除");
        _coin.Tags?.ClearAll();
        _coin.CallDeferred("queue_free");
    }
}

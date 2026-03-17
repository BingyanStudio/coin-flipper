using System;
using Godot;

/// <summary>
/// 【虚无】标签：回合结束后硬币消失。
/// 响应 RoundEndEvent，回合结束时移除宿主硬币。
/// 
/// 触发时机：回合结束（RoundEndEvent）
/// 效果：局结束后硬币从场地上消失
/// </summary>
public class VoidTag : ITagBehavior
{
    public string TagId => TagIds.Void;

    private Action _unsubscribe;
    private Coin _coin;

    public void OnAttached(TagContext context)
    {
        _coin = context.Coin;
        _unsubscribe = context.EventBus.Subscribe<RoundEndEvent>(OnRoundEnd);
    }

    public void OnDetached(TagContext context)
    {
        _unsubscribe?.Invoke();
        _unsubscribe = null;
    }

    private void OnRoundEnd(RoundEndEvent evt)
    {
        GD.Print($"[VoidTag] {_coin.Name}: 虚无硬币在回合结束后消失");
        _coin.Tags?.ClearAll();
        _coin.CallDeferred("queue_free");
    }
}

using System;
using Godot;

/// <summary>
/// 【生物】标签：被砸死后变尸体。
/// 响应 CoinHitEvent，当宿主硬币被其他硬币砸中时，
/// 移除【生物】标签并添加【尸体】标签。
/// 
/// 这是一个通用机制：任何带有【生物】标签的硬币被砸死时，
/// 都会自动执行"移除生物 + 添加尸体"的状态转换。
/// 
/// 触发时机：OnHit
/// 效果：被砸 → 移除【生物】标签 + 添加【尸体】标签
/// </summary>
public class CreatureTag : ITagBehavior
{
    public string TagId => TagIds.Creature;

    private Action _unsubscribe;
    private Coin _coin;
    private TagManager _tagManager;

    public void OnAttached(TagContext context)
    {
        _coin = context.Coin;
        _tagManager = context.TagManager;
        _unsubscribe = context.EventBus.Subscribe<CoinHitEvent>(OnCoinHit);
    }

    public void OnDetached(TagContext context)
    {
        _unsubscribe?.Invoke();
        _unsubscribe = null;
    }

    private void OnCoinHit(CoinHitEvent evt)
    {
        if (evt.Target != _coin) return;

        GD.Print($"[CreatureTag] {_coin.Name}: 生物被 {evt.Hitter?.Name} 砸死，变为尸体");

        // 移除生物标签，添加尸体标签
        _tagManager.RemoveTag(TagIds.Creature);
        _tagManager.AddIntrinsicTag(new CorpseTag());
    }
}

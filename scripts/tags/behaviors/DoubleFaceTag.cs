using System;
using Godot;

/// <summary>
/// 【正面翻倍】标签：正面朝上得分翻倍，背面朝上不拿分。
/// 结算时产出 ScoreModifier，由结算管线收集并执行。
/// 
/// 触发时机：OnScore
/// 效果：
/// - 正面朝上：该硬币的 Chips 贡献 ×2
/// - 背面朝上：该硬币的 Chips 贡献 = 0
/// </summary>
public class DoubleFaceTag : ITagBehavior
{
    public string TagId => TagIds.DoubleFace;

    private Action _unsubscribe;
    private Coin _coin;

    public void OnAttached(TagContext context)
    {
        _coin = context.Coin;
        _unsubscribe = context.EventBus.Subscribe<ScorePhaseEvent>(OnScorePhase);
    }

    public void OnDetached(TagContext context)
    {
        _unsubscribe?.Invoke();
        _unsubscribe = null;
    }

    private void OnScorePhase(ScorePhaseEvent evt)
    {
        if (evt.Coin != _coin) return;

        // 结算逻辑将在 Milestone 3 的结算管线中完善
        // 目前仅记录日志，标记效果
        if (_coin.IsFaceUp)
        {
            GD.Print($"[DoubleFaceTag] {_coin.Name}: 正面朝上，得分翻倍！");
        }
        else
        {
            GD.Print($"[DoubleFaceTag] {_coin.Name}: 背面朝上，不拿分");
        }
    }

    /// <summary>
    /// 获取该标签对 Chips 的乘数修饰。
    /// 由结算管线在计分时调用。
    /// </summary>
    public float GetChipsMultiplier()
    {
        if (_coin == null) return 1f;
        return _coin.IsFaceUp ? 2f : 0f;
    }
}

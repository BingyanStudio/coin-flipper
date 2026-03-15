using System;
using Godot;

/// <summary>
/// 【尸体】标签：面值归零。
/// 被动属性修饰标签，不响应事件，通过 Coin.GetEffectiveFaceValue() 生效。
/// 
/// 触发时机：Passive
/// 效果：硬币面值视为 0
/// 来源：生物被砸死后自动添加
/// </summary>
public class CorpseTag : ITagBehavior
{
    public string TagId => TagIds.Corpse;

    public void OnAttached(TagContext context)
    {
        // 被动标签，无需订阅事件
        // 面值归零通过 Coin.GetEffectiveFaceValue() 查询时判断
    }

    public void OnDetached(TagContext context) { }
}

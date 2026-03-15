using System;
using Godot;

/// <summary>
/// 【超重】标签：质量 ×10。
/// 被动属性修饰标签，附着时修改硬币物理质量，移除时恢复。
/// 
/// 触发时机：Passive
/// 效果：硬币质量变为原来的 10 倍
/// </summary>
public class HeavyTag : ITagBehavior
{
    public string TagId => TagIds.Heavy;

    private float _originalMass;

    public void OnAttached(TagContext context)
    {
        _originalMass = context.Coin.Mass;
        context.Coin.Mass = _originalMass * 10f;
        GD.Print($"[HeavyTag] {context.Coin.Name}: 质量 {_originalMass} → {context.Coin.Mass}");
    }

    public void OnDetached(TagContext context)
    {
        context.Coin.Mass = _originalMass;
        GD.Print($"[HeavyTag] {context.Coin.Name}: 质量恢复为 {_originalMass}");
    }
}

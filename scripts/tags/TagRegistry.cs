using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// 标签行为注册表。根据 TagId 创建标签行为实例。
/// 所有标签行为类型在此集中注册，避免散落的 new 调用。
/// 
/// 服务于 Milestone 2：标签系统核心。
/// </summary>
public static class TagRegistry
{
    /// <summary>标签工厂函数映射：TagId → 创建函数</summary>
    private static readonly Dictionary<string, Func<ITagBehavior>> _factories = new()
    {
        { TagIds.Void, () => new VoidTag() },
        { TagIds.Fragile, () => new FragileTag() },
        { TagIds.Creature, () => new CreatureTag() },
        { TagIds.Corpse, () => new CorpseTag() },
        { TagIds.Heavy, () => new HeavyTag() },
        { TagIds.Return, () => new ReturnTag() },
        { TagIds.Bounce, () => new BounceTag() },
        { TagIds.DoubleFace, () => new DoubleFaceTag() },
        { TagIds.Magnetic, () => new MagneticTag() },
        { TagIds.Rabbit, () => new RabbitTag() },
    };

    /// <summary>
    /// 根据 TagId 创建标签行为实例。
    /// </summary>
    /// <param name="tagId">标签 ID</param>
    /// <returns>标签行为实例，未注册时返回 null</returns>
    public static ITagBehavior Create(string tagId)
    {
        if (_factories.TryGetValue(tagId, out var factory))
            return factory();

        GD.PrintErr($"[TagRegistry] 未注册的标签 ID: {tagId}");
        return null;
    }

    /// <summary>检查指定 TagId 是否已注册</summary>
    public static bool IsRegistered(string tagId)
    {
        return _factories.ContainsKey(tagId);
    }
}

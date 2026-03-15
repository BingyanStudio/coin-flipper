using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// 标签管理器。管理硬币上标签的附着、移除和查询。
/// 每个硬币拥有一个 TagManager 实例。
/// 
/// 职责：
/// - 管理固有标签（不受品质槽位限制）和附着标签（受品质槽位限制）
/// - 标签生命周期管理（OnAttached / OnDetached）
/// - 按 TagId 查询标签
/// - 提供标签列表的只读访问
/// 
/// 服务于 Milestone 2：标签系统核心。
/// </summary>
public class TagManager
{
    private readonly Coin _coin;
    private readonly EventBus _eventBus;

    /// <summary>固有标签列表（不受品质槽位限制）</summary>
    private readonly List<TagInstance> _intrinsicTags = new();

    /// <summary>玩家附着的标签列表（受品质槽位限制）</summary>
    private readonly List<TagInstance> _attachedTags = new();

    /// <summary>硬币品质，决定可附着标签的最大数量</summary>
    public CoinQuality Quality { get; set; } = CoinQuality.Normal;

    /// <summary>所有标签（固有 + 附着）的只读视图</summary>
    public IReadOnlyList<TagInstance> AllTags
    {
        get
        {
            var all = new List<TagInstance>(_intrinsicTags.Count + _attachedTags.Count);
            all.AddRange(_intrinsicTags);
            all.AddRange(_attachedTags);
            return all;
        }
    }

    /// <summary>当前附着标签数量</summary>
    public int AttachedCount => _attachedTags.Count;

    /// <summary>最大可附着标签数量（由品质决定）</summary>
    public int MaxAttachSlots => (int)Quality;

    public TagManager(Coin coin, EventBus eventBus)
    {
        _coin = coin;
        _eventBus = eventBus;
    }

    /// <summary>
    /// 添加固有标签（不受槽位限制）。
    /// 用于硬币定义中的固有标签。
    /// </summary>
    public void AddIntrinsicTag(ITagBehavior behavior)
    {
        var instance = new TagInstance(behavior, isIntrinsic: true);
        _intrinsicTags.Add(instance);
        behavior.OnAttached(CreateContext());
        GD.Print($"[TagManager] {_coin.Name}: 添加固有标签 [{behavior.TagId}]");
    }

    /// <summary>
    /// 附着标签（受品质槽位限制）。
    /// </summary>
    /// <returns>是否成功附着（槽位已满时返回 false）</returns>
    public bool AttachTag(ITagBehavior behavior)
    {
        if (_attachedTags.Count >= MaxAttachSlots)
        {
            GD.Print($"[TagManager] {_coin.Name}: 标签槽已满 ({AttachedCount}/{MaxAttachSlots})，无法附着 [{behavior.TagId}]");
            return false;
        }

        var instance = new TagInstance(behavior, isIntrinsic: false);
        _attachedTags.Add(instance);
        behavior.OnAttached(CreateContext());
        GD.Print($"[TagManager] {_coin.Name}: 附着标签 [{behavior.TagId}] ({AttachedCount}/{MaxAttachSlots})");
        return true;
    }

    /// <summary>
    /// 移除指定 TagId 的第一个标签（优先移除附着标签）。
    /// </summary>
    /// <returns>是否成功移除</returns>
    public bool RemoveTag(string tagId)
    {
        // 先尝试从附着标签中移除
        var attached = _attachedTags.FirstOrDefault(t => t.Behavior.TagId == tagId);
        if (attached != null)
        {
            attached.Behavior.OnDetached(CreateContext());
            _attachedTags.Remove(attached);
            GD.Print($"[TagManager] {_coin.Name}: 移除附着标签 [{tagId}]");
            return true;
        }

        // 再尝试从固有标签中移除
        var intrinsic = _intrinsicTags.FirstOrDefault(t => t.Behavior.TagId == tagId);
        if (intrinsic != null)
        {
            intrinsic.Behavior.OnDetached(CreateContext());
            _intrinsicTags.Remove(intrinsic);
            GD.Print($"[TagManager] {_coin.Name}: 移除固有标签 [{tagId}]");
            return true;
        }

        return false;
    }

    /// <summary>查询是否拥有指定 TagId 的标签</summary>
    public bool HasTag(string tagId)
    {
        return AllTags.Any(t => t.Behavior.TagId == tagId);
    }

    /// <summary>获取指定 TagId 的所有标签实例</summary>
    public IEnumerable<TagInstance> GetTags(string tagId)
    {
        return AllTags.Where(t => t.Behavior.TagId == tagId);
    }

    /// <summary>移除所有标签并调用 OnDetached</summary>
    public void ClearAll()
    {
        var context = CreateContext();
        foreach (var tag in _intrinsicTags)
            tag.Behavior.OnDetached(context);
        foreach (var tag in _attachedTags)
            tag.Behavior.OnDetached(context);
        _intrinsicTags.Clear();
        _attachedTags.Clear();
    }

    private TagContext CreateContext()
    {
        return new TagContext
        {
            Coin = _coin,
            EventBus = _eventBus,
            TagManager = this,
        };
    }
}

/// <summary>
/// 标签实例。包装 ITagBehavior 并记录元数据（是否固有等）。
/// </summary>
public class TagInstance
{
    public ITagBehavior Behavior { get; }
    public bool IsIntrinsic { get; }

    public TagInstance(ITagBehavior behavior, bool isIntrinsic)
    {
        Behavior = behavior;
        IsIntrinsic = isIntrinsic;
    }
}

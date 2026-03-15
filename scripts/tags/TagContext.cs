/// <summary>
/// 标签运行上下文。提供标签与游戏系统交互的接口。
/// 标签不直接访问 Godot 节点树，而是通过此上下文间接操作。
/// 
/// 职责：
/// - 提供对宿主硬币的引用
/// - 提供事件总线访问
/// - 提供标签管理器访问（用于添加/移除标签）
/// </summary>
public class TagContext
{
    /// <summary>标签所附着的硬币</summary>
    public Coin Coin { get; init; }

    /// <summary>全局事件总线</summary>
    public EventBus EventBus { get; init; }

    /// <summary>标签管理器（用于动态添加/移除标签）</summary>
    public TagManager TagManager { get; init; }
}

/// <summary>
/// 标签行为接口。所有标签的行为逻辑必须实现此接口。
/// 标签是纯 C# 对象（非 Godot Node），保证轻量和可测试性。
/// 
/// 设计原则：
/// - 标签之间不直接引用，通过事件和共享状态间接交互
/// - 标签只依赖：硬币的公共接口、游戏事件、TagContext
/// - 效果是数据（ScoreModifier），执行由结算管线负责
/// 
/// 服务于 Milestone 2：标签系统核心。
/// </summary>
public interface ITagBehavior
{
    /// <summary>
    /// 标签的唯一标识符（如 "fragile", "void", "creature"）。
    /// 用于数据驱动引用和标签管理器查询。
    /// </summary>
    string TagId { get; }

    /// <summary>
    /// 标签被附着到硬币上时调用。
    /// 用于初始化状态、订阅事件等。
    /// </summary>
    /// <param name="context">标签运行上下文，提供对硬币和事件总线的访问</param>
    void OnAttached(TagContext context);

    /// <summary>
    /// 标签从硬币上移除时调用。
    /// 用于清理状态、取消事件订阅等。
    /// </summary>
    /// <param name="context">标签运行上下文</param>
    void OnDetached(TagContext context);
}

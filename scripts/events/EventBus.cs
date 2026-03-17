using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// 中心化事件总线。所有游戏事件通过此总线分发。
/// 标签系统的核心通信机制——物理层产生事件，标签层消费事件。
/// 
/// 特性：
/// - 类型安全的泛型订阅/发布
/// - 深度计数器防止无限循环（最大深度 10）
/// - 支持取消订阅（返回 Action 用于清理）
/// 
/// 服务于 Milestone 2：标签系统核心。
/// </summary>
public class EventBus
{
    /// <summary>事件分发的最大递归深度，超过则中断并警告</summary>
    private const int MaxDepth = 10;

    /// <summary>当前分发深度</summary>
    private int _currentDepth;

    /// <summary>
    /// 按事件类型存储的订阅者列表。
    /// Key = typeof(TEvent), Value = List of Action&lt;GameEvent&gt; wrappers
    /// </summary>
    private readonly Dictionary<Type, List<Action<GameEvent>>> _handlers = new();

    /// <summary>
    /// 订阅指定类型的事件。
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="handler">事件处理函数</param>
    /// <returns>取消订阅的 Action，调用即可移除此订阅</returns>
    public Action Subscribe<T>(Action<T> handler) where T : GameEvent
    {
        var type = typeof(T);
        if (!_handlers.ContainsKey(type))
            _handlers[type] = new List<Action<GameEvent>>();

        // 包装为 Action<GameEvent> 以统一存储
        Action<GameEvent> wrapper = e => handler((T)e);
        _handlers[type].Add(wrapper);

        // 返回取消订阅的闭包
        return () => _handlers[type].Remove(wrapper);
    }

    /// <summary>
    /// 发布事件，通知所有订阅者。
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="evt">事件实例</param>
    public void Publish<T>(T evt) where T : GameEvent
    {
        _currentDepth++;
        evt.Depth = _currentDepth;

        if (_currentDepth > MaxDepth)
        {
            GD.PrintErr($"[EventBus] 事件递归深度超过 {MaxDepth}，中断分发: {typeof(T).Name}");
            _currentDepth--;
            return;
        }

        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var handlers))
        {
            // 复制列表以防止迭代中修改
            var snapshot = new List<Action<GameEvent>>(handlers);
            foreach (var handler in snapshot)
            {
                try
                {
                    handler(evt);
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"[EventBus] 处理 {type.Name} 时异常: {ex.Message}");
                }
            }
        }

        _currentDepth--;
    }

    /// <summary>清除所有订阅（用于场景切换等清理场景）</summary>
    public void Clear()
    {
        _handlers.Clear();
        _currentDepth = 0;
    }
}

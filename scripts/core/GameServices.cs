/// <summary>
/// 全局游戏服务定位器。提供对核心系统的静态访问。
/// 在 GameManager._Ready() 中初始化。
/// 
/// 注意：仅用于真正的全局服务（EventBus 等），
/// 不要滥用为万能容器。
/// </summary>
public static class GameServices
{
    /// <summary>全局事件总线实例</summary>
    public static EventBus EventBus { get; private set; }

    /// <summary>初始化所有全局服务（由 GameManager 调用）</summary>
    public static void Initialize()
    {
        EventBus = new EventBus();
    }

    /// <summary>清理所有全局服务（场景切换时调用）</summary>
    public static void Cleanup()
    {
        EventBus?.Clear();
    }
}

using System;
using Godot;

/// <summary>
/// 【回归】标签：掉出场地后回到场地中心。
/// 响应 CoinFellOffEvent，将硬币传送回场地中心上方。
/// 
/// 触发时机：OnFallOff
/// 效果：硬币掉出场地后，传送回场地中心上方并重新落下
/// </summary>
public class ReturnTag : ITagBehavior
{
    public string TagId => TagIds.Return;

    /// <summary>回归时硬币出现在场地中心上方的高度</summary>
    private const float ReturnHeight = 3f;

    private Action _unsubscribe;
    private Coin _coin;

    public void OnAttached(TagContext context)
    {
        _coin = context.Coin;
        _unsubscribe = context.EventBus.Subscribe<CoinFellOffEvent>(OnFellOff);
    }

    public void OnDetached(TagContext context)
    {
        _unsubscribe?.Invoke();
        _unsubscribe = null;
    }

    private void OnFellOff(CoinFellOffEvent evt)
    {
        if (evt.Coin != _coin) return;

        // 回归到场地中心上方，避免回归到边缘导致重复掉落
        Vector3 returnPos = new Vector3(0f, ReturnHeight, 0f);

        GD.Print($"[ReturnTag] {_coin.Name}: 回归到场地中心 ({returnPos.X:F1}, {returnPos.Y:F1}, {returnPos.Z:F1})");

        // 重置速度并传送
        _coin.LinearVelocity = Vector3.Zero;
        _coin.AngularVelocity = Vector3.Zero;
        _coin.GlobalPosition = returnPos;
    }
}

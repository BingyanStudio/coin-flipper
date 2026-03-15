using System;
using Godot;

/// <summary>
/// 【兔子】标签：落地后生成小兔子硬币。
/// 响应 CoinSettledEvent，在硬币静止后生成 x 只小兔子硬币，
/// 其中 x = 本次翻转次数。
/// 
/// 触发时机：OnLand
/// 效果：硬币落地后，在场地上生成小兔子硬币
/// 依赖：CoinFactory（用于生成小兔子硬币实例）
/// </summary>
public class RabbitTag : ITagBehavior
{
    public string TagId => TagIds.Rabbit;

    /// <summary>生成小兔子的散布半径</summary>
    private const float SpawnRadius = 1.5f;

    /// <summary>生成小兔子的高度</summary>
    private const float SpawnHeight = 2f;

    private Action _unsubFlip;
    private Action _unsubSettle;
    private Coin _coin;
    private EventBus _eventBus;
    private bool _canSpawn;

    public void OnAttached(TagContext context)
    {
        _coin = context.Coin;
        _eventBus = context.EventBus;
        _unsubFlip = context.EventBus.Subscribe<CoinFlippedEvent>(OnFlipped);
        _unsubSettle = context.EventBus.Subscribe<CoinSettledEvent>(OnSettled);
    }

    public void OnDetached(TagContext context)
    {
        _unsubFlip?.Invoke();
        _unsubSettle?.Invoke();
        _unsubFlip = null;
        _unsubSettle = null;
    }

    private void OnFlipped(CoinFlippedEvent evt)
    {
        if (evt.Coin != _coin) return;
        _canSpawn = true;
    }

    private void OnSettled(CoinSettledEvent evt)
    {
        if (evt.Coin != _coin || !_canSpawn) return;
        _canSpawn = false;

        int count = Mathf.Max(1, evt.FlipCount);
        GD.Print($"[RabbitTag] {_coin.Name}: 生成 {count} 只小兔子（翻转次数={evt.FlipCount}）");

        // 通过 CoinFactory 生成小兔子硬币
        var container = _coin.GetParent();
        if (container == null) return;

        for (int i = 0; i < count; i++)
        {
            var bunny = CoinFactory.CreateBunnyCoin();
            if (bunny == null)
            {
                GD.PrintErr("[RabbitTag] CoinFactory.CreateBunnyCoin() 返回 null");
                break;
            }

            // 在母体周围随机位置生成
            float angle = (float)GD.RandRange(0, Mathf.Tau);
            float radius = (float)GD.RandRange(0.3f, SpawnRadius);
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * radius,
                SpawnHeight,
                Mathf.Sin(angle) * radius
            );

            container.AddChild(bunny);
            bunny.GlobalPosition = _coin.GlobalPosition + offset;

            _eventBus?.Publish(new CoinSpawnedEvent { Coin = bunny });
        }
    }
}

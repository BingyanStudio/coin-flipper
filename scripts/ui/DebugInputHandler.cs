using Godot;

/// <summary>
/// 调试快捷键处理器。处理物理参数调节和标签测试输入。
/// 与 DebugUI（纯显示）分离，避免 UI 层直接承担输入职责。
/// 由 DebugBootstrap 创建并挂载。
/// 
/// 快捷键：
/// - +/-: 调整基础力度
/// - [/]: 调整精准度
/// - 1: 生成小兔子硬币
/// - 2: 生成黑曜石硬币
/// - 3: 生成回归硬币（带回归标签）
/// - 4: 生成磁力硬币
/// - 5: 生成弹跳硬币
/// </summary>
public partial class DebugInputHandler : Node
{
    private const float ForceStep = 1f;
    private const float PrecisionStep = 0.1f;

    private CoinFlipper _flipper;
    private Node _coinContainer;

    public override void _Ready()
    {
        _flipper = GetTree().Root.FindChild(
            SceneNodes.CoinFlipper, true, false) as CoinFlipper;
        _coinContainer = GetTree().Root.FindChild(
            SceneNodes.CoinContainer, true, false);

        if (_flipper == null)
            GD.PrintErr($"[DebugInputHandler] 未找到节点 '{SceneNodes.CoinFlipper}'，快捷键调参不可用");
        if (_coinContainer == null)
            GD.PrintErr($"[DebugInputHandler] 未找到节点 '{SceneNodes.CoinContainer}'，硬币生成不可用");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventKey key || !key.Pressed) return;

        // 力度和精准度调节
        if (_flipper != null)
        {
            switch (key.Keycode)
            {
                case Key.Equal:
                    _flipper.BaseForce += ForceStep;
                    return;
                case Key.Minus:
                    _flipper.BaseForce -= ForceStep;
                    return;
                case Key.Bracketright:
                    _flipper.Precision = Mathf.Min(
                        1f, _flipper.Precision + PrecisionStep);
                    return;
                case Key.Bracketleft:
                    _flipper.Precision = Mathf.Max(
                        0f, _flipper.Precision - PrecisionStep);
                    return;
            }
        }

        // 标签测试：生成特殊硬币
        if (_coinContainer != null)
        {
            switch (key.Keycode)
            {
                case Key.Key1:
                    SpawnDebugCoin(CoinFactory.CreateBunnyCoin(), "bunny");
                    return;
                case Key.Key2:
                    SpawnDebugCoin(CoinFactory.CreateObsidianCoin(), "obsidian");
                    return;
                case Key.Key3:
                    SpawnDebugCoinWithTag(TagIds.Return, "回归币");
                    return;
                case Key.Key4:
                    SpawnDebugCoinWithTag(TagIds.Magnetic, "磁力币");
                    return;
                case Key.Key5:
                    SpawnDebugCoinWithTag(TagIds.Bounce, "弹跳币");
                    return;
            }
        }
    }

    /// <summary>生成预定义的特殊硬币</summary>
    private void SpawnDebugCoin(Coin coin, string defId)
    {
        if (coin == null) return;

        // 在场地中心上方随机位置生成
        float x = (float)GD.RandRange(-2, 2);
        float z = (float)GD.RandRange(-2, 2);

        _coinContainer.AddChild(coin);
        coin.GlobalPosition = new Vector3(x, 3f, z);

        // 硬币已添加到场景树，_Ready() 已执行，现在可以附着标签
        var definition = defId switch
        {
            "bunny" => new CoinDefinition
            {
                IntrinsicTagIds = new[] { TagIds.Void, TagIds.Fragile, TagIds.Creature }
            },
            "obsidian" => new CoinDefinition
            {
                IntrinsicTagIds = new[] { TagIds.Void, TagIds.Heavy }
            },
            _ => new CoinDefinition { IntrinsicTagIds = System.Array.Empty<string>() }
        };

        CoinFactory.ApplyIntrinsicTags(coin, definition);
        GameServices.EventBus?.Publish(new CoinSpawnedEvent { Coin = coin });

        GD.Print($"[Debug] 生成 {coin.Name} at ({x:F1}, 3.0, {z:F1})");
    }

    /// <summary>生成带有单个标签的普通硬币</summary>
    private void SpawnDebugCoinWithTag(string tagId, string name)
    {
        var definition = new CoinDefinition
        {
            DefinitionId = $"debug_{tagId}",
            DisplayName = name,
            FaceValue = 1,
            Quality = CoinQuality.Normal,
            SizeScale = 1.0f,
            Mass = 0.5f,
            IntrinsicTagIds = new[] { tagId },
            AlbedoColor = tagId switch
            {
                TagIds.Return => new Color(0.2f, 0.8f, 0.3f, 1f),   // 绿色
                TagIds.Magnetic => new Color(0.8f, 0.2f, 0.2f, 1f), // 红色
                TagIds.Bounce => new Color(0.2f, 0.5f, 0.9f, 1f),   // 蓝色
                _ => new Color(0.85f, 0.65f, 0.13f, 1f),
            },
        };

        var coin = CoinFactory.CreateCoin(definition);
        if (coin == null) return;

        float x = (float)GD.RandRange(-2, 2);
        float z = (float)GD.RandRange(-2, 2);

        _coinContainer.AddChild(coin);
        coin.GlobalPosition = new Vector3(x, 3f, z);

        CoinFactory.ApplyIntrinsicTags(coin, definition);
        GameServices.EventBus?.Publish(new CoinSpawnedEvent { Coin = coin });

        GD.Print($"[Debug] 生成 {name} [{tagId}] at ({x:F1}, 3.0, {z:F1})");
    }
}

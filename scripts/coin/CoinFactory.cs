using Godot;

/// <summary>
/// 硬币工厂。根据 CoinDefinition 生成硬币实例。
/// 负责：加载硬币场景模板 → 设置属性 → 附着固有标签。
/// 
/// 服务于 Milestone 2：标签系统核心。
/// </summary>
public static class CoinFactory
{
    /// <summary>硬币场景模板路径</summary>
    private const string CoinScenePath = "res://scenes/coin/Coin.tscn";

    /// <summary>缓存的硬币场景模板</summary>
    private static PackedScene _coinScene;

    /// <summary>
    /// 根据 CoinDefinition 创建硬币实例。
    /// </summary>
    /// <param name="definition">硬币定义</param>
    /// <returns>配置好的硬币实例（尚未添加到场景树）</returns>
    public static Coin CreateCoin(CoinDefinition definition)
    {
        var coin = InstantiateCoinScene();
        if (coin == null) return null;

        // 设置基础属性
        coin.Name = definition.DisplayName;
        coin.FaceValue = definition.FaceValue;
        coin.Quality = definition.Quality;
        coin.Mass = definition.Mass;

        // 设置大小缩放
        if (!Mathf.IsEqualApprox(definition.SizeScale, 1.0f))
        {
            coin.Scale = Vector3.One * definition.SizeScale;
        }

        // 设置颜色（临时方案）
        var mesh = coin.GetNodeOrNull<MeshInstance3D>("MeshInstance3D");
        if (mesh != null)
        {
            var material = new StandardMaterial3D();
            material.AlbedoColor = definition.AlbedoColor;
            material.Metallic = 0.8f;
            material.Roughness = 0.3f;
            mesh.SetSurfaceOverrideMaterial(0, material);
        }

        return coin;
    }

    /// <summary>
    /// 为硬币附着固有标签。必须在硬币添加到场景树并调用 _Ready() 之后调用，
    /// 因为 Tags (TagManager) 在 _Ready() 中初始化。
    /// </summary>
    /// <param name="coin">已添加到场景树的硬币</param>
    /// <param name="definition">硬币定义</param>
    public static void ApplyIntrinsicTags(Coin coin, CoinDefinition definition)
    {
        if (coin.Tags == null)
        {
            GD.PrintErr($"[CoinFactory] {coin.Name}: Tags 未初始化，请确保硬币已添加到场景树");
            return;
        }

        coin.Tags.Quality = definition.Quality;

        foreach (var tagId in definition.IntrinsicTagIds)
        {
            var behavior = TagRegistry.Create(tagId);
            if (behavior != null)
            {
                coin.Tags.AddIntrinsicTag(behavior);
            }
        }
    }

    /// <summary>
    /// 创建小兔子硬币（快捷方法）。
    /// 小兔子 = 面值1 + 极小 + {虚无, 脆弱, 生物}
    /// </summary>
    public static Coin CreateBunnyCoin()
    {
        var definition = new CoinDefinition
        {
            DefinitionId = "bunny",
            DisplayName = "小兔子",
            FaceValue = 1,
            Quality = CoinQuality.Normal,
            SizeScale = 0.4f,
            Mass = 0.1f,
            IntrinsicTagIds = new[] { TagIds.Void, TagIds.Fragile, TagIds.Creature },
            AlbedoColor = new Color(0.9f, 0.8f, 0.7f, 1f), // 浅棕色
        };

        return CreateCoin(definition);
    }

    /// <summary>
    /// 创建黑曜石硬币（Boss 盲注用，快捷方法）。
    /// 黑曜石 = 面值10 + 中等 + {虚无, 超重}
    /// </summary>
    public static Coin CreateObsidianCoin()
    {
        var definition = new CoinDefinition
        {
            DefinitionId = "obsidian",
            DisplayName = "黑曜石",
            FaceValue = 10,
            Quality = CoinQuality.Normal,
            SizeScale = 1.0f,
            Mass = 0.5f,
            IntrinsicTagIds = new[] { TagIds.Void, TagIds.Heavy },
            AlbedoColor = new Color(0.15f, 0.1f, 0.2f, 1f), // 深紫黑色
        };

        return CreateCoin(definition);
    }

    /// <summary>实例化硬币场景模板</summary>
    private static Coin InstantiateCoinScene()
    {
        if (_coinScene == null)
        {
            _coinScene = GD.Load<PackedScene>(CoinScenePath);
            if (_coinScene == null)
            {
                GD.PrintErr($"[CoinFactory] 无法加载硬币场景: {CoinScenePath}");
                return null;
            }
        }

        var instance = _coinScene.Instantiate();
        if (instance is Coin coin)
            return coin;

        GD.PrintErr("[CoinFactory] 硬币场景根节点不是 Coin 类型");
        instance.QueueFree();
        return null;
    }
}

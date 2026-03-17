using Godot;

/// <summary>
/// 硬币定义资源。数据驱动的硬币模板，定义硬币的基础属性和固有标签。
/// 通过 Godot Resource 系统实现，可在编辑器中创建和编辑 .tres 文件。
/// 
/// 硬币 = 基础属性（面值、大小、品质）+ 固有标签列表
/// 
/// 服务于 Milestone 2：标签系统核心。
/// </summary>
[GlobalClass]
public partial class CoinDefinition : Resource
{
    /// <summary>硬币定义的唯一 ID（如 "bunny", "obsidian"）</summary>
    [Export] public string DefinitionId { get; set; } = "";

    /// <summary>硬币显示名称</summary>
    [Export] public string DisplayName { get; set; } = "硬币";

    /// <summary>硬币面值</summary>
    [Export] public int FaceValue { get; set; } = 1;

    /// <summary>硬币品质</summary>
    [Export] public CoinQuality Quality { get; set; } = CoinQuality.Normal;

    /// <summary>硬币大小缩放（1.0 = 标准大小）</summary>
    [Export] public float SizeScale { get; set; } = 1.0f;

    /// <summary>硬币质量（物理属性）</summary>
    [Export] public float Mass { get; set; } = 0.5f;

    /// <summary>
    /// 固有标签 ID 列表。这些标签在硬币创建时自动附着，不受品质槽位限制。
    /// </summary>
    [Export] public string[] IntrinsicTagIds { get; set; } = System.Array.Empty<string>();

    /// <summary>硬币颜色（临时，后续替换为纹理/材质）</summary>
    [Export] public Color AlbedoColor { get; set; } = new Color(0.85f, 0.65f, 0.13f, 1f);
}

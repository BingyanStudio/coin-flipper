/// <summary>
/// 标签 ID 常量。集中定义所有标签的字符串标识符，
/// 避免魔法字符串散落在各处。
/// </summary>
public static class TagIds
{
    /// <summary>虚无：局结束后消失</summary>
    public const string Void = "void";

    /// <summary>脆弱：翻转后消失</summary>
    public const string Fragile = "fragile";

    /// <summary>生物：被砸死后变尸体</summary>
    public const string Creature = "creature";

    /// <summary>尸体：面值归零</summary>
    public const string Corpse = "corpse";

    /// <summary>超重：质量 ×10</summary>
    public const string Heavy = "heavy";

    /// <summary>回归：掉出场地后回到场地</summary>
    public const string Return = "return";

    /// <summary>再弹跳：落地后再弹跳一次</summary>
    public const string Bounce = "bounce";

    /// <summary>正面翻倍：正面朝上得分翻倍，背面不拿分</summary>
    public const string DoubleFace = "double_face";

    /// <summary>磁力：落地后吸引周围硬币</summary>
    public const string Magnetic = "magnetic";

    /// <summary>兔子：落地后生成小兔子硬币</summary>
    public const string Rabbit = "rabbit";
}

/// <summary>
/// 硬币品质/稀有度枚举。决定硬币可附着的标签槽数量。
/// </summary>
public enum CoinQuality
{
    /// <summary>普通品质，最多附着 1 个标签</summary>
    Normal = 1,

    /// <summary>稀有品质，最多附着 2 个标签</summary>
    Rare = 2,

    /// <summary>传说品质，最多附着 3 个标签</summary>
    Legendary = 3,
}

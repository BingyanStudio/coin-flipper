using Godot;
using System.Linq;
using System.Text;

/// <summary>
/// 调试信息面板。实时显示硬币状态和物理参数。
/// 纯显示职责，输入处理由 DebugInputHandler 负责。
/// 服务于 Milestone 1：物理手感调试。
/// </summary>
public partial class DebugUI : Control
{
    /// <summary>面板刷新间隔（秒），避免每帧字符串拼接</summary>
    private const float UpdateInterval = 0.1f;

    private Label _infoLabel;
    private CoinFlipper _flipper;
    private Node _coinContainer;
    private readonly StringBuilder _sb = new();
    private float _updateTimer;

    public override void _Ready()
    {
        _infoLabel = new Label();
        _infoLabel.Position = new Vector2(10, 10);
        _infoLabel.AddThemeColorOverride("font_color", Colors.White);
        _infoLabel.AddThemeFontSizeOverride("font_size", 14);
        AddChild(_infoLabel);

        _flipper = GetTree().Root.FindChild(
            SceneNodes.CoinFlipper, true, false) as CoinFlipper;
        _coinContainer = GetTree().Root.FindChild(
            SceneNodes.CoinContainer, true, false);

        if (_flipper == null)
            GD.PrintErr($"[DebugUI] 未找到节点 '{SceneNodes.CoinFlipper}'，力度/精准度信息不可用");
        if (_coinContainer == null)
            GD.PrintErr($"[DebugUI] 未找到节点 '{SceneNodes.CoinContainer}'，硬币状态信息不可用");
    }

    public override void _Process(double delta)
    {
        _updateTimer += (float)delta;
        if (_updateTimer < UpdateInterval) return;
        _updateTimer = 0f;

        var coins = _coinContainer?.GetChildren()
            .OfType<Coin>().ToArray() ?? System.Array.Empty<Coin>();

        _sb.Clear();
        _sb.AppendLine("[调试面板]");
        _sb.AppendLine($"硬币数量: {coins.Length}");
        if (_flipper != null)
        {
            _sb.AppendLine($"基础力度: {_flipper.BaseForce:F1}");
            _sb.AppendLine($"浮动范围: {_flipper.ForceVariance:P0}");
            _sb.AppendLine($"精准度: {_flipper.Precision:P0}");
        }
        _sb.AppendLine("---");

        foreach (var coin in coins)
        {
            string state = coin.IsSettled ? "静止" : "运动";
            string face = coin.IsFaceUp ? "正面" : "反面";
            string standing = coin.IsStanding ? " [立起来!]" : "";
            _sb.Append($"{coin.Name}: {state} {face}{standing}");
            _sb.Append($" 翻转={coin.FlipCount}次");
            _sb.AppendLine($" 面值={coin.FaceValue}");
        }

        _infoLabel.Text = _sb.ToString();
    }
}

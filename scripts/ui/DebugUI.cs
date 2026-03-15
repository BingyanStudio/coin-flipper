using Godot;
using System.Linq;

/// <summary>
/// 调试信息面板。实时显示硬币状态和物理参数调节。
/// 服务于 Milestone 1：物理手感调试。
/// </summary>
public partial class DebugUI : Control
{
    // 力度调节步进
    private const float ForceStep = 1f;
    // 精准度调节步进
    private const float PrecisionStep = 0.1f;
    private Label _infoLabel;
    private CoinFlipper _flipper;

    public override void _Ready()
    {
        // 创建信息标签
        _infoLabel = new Label();
        _infoLabel.Position = new Vector2(10, 10);
        _infoLabel.AddThemeColorOverride("font_color", Colors.White);
        _infoLabel.AddThemeFontSizeOverride("font_size", 14);
        AddChild(_infoLabel);

        _flipper = GetTree().Root.FindChild("CoinFlipper",
            true, false) as CoinFlipper;
    }

    public override void _Process(double delta)
    {
        var coins = GetTree().GetNodesInGroup("coins")
            .OfType<Coin>().ToArray();
        if (coins.Length == 0)
        {
            // 如果没有分组，尝试从 CoinContainer 获取
            var container = GetTree().Root.FindChild(
                "CoinContainer", true, false);
            if (container != null)
            {
                coins = container.GetChildren()
                    .OfType<Coin>().ToArray();
            }
        }

        string info = "[调试面板]\n";
        info += $"硬币数量: {coins.Length}\n";
        if (_flipper != null)
        {
            info += $"基础力度: {_flipper.BaseForce:F1}\n";
            info += $"浮动范围: {_flipper.ForceVariance:P0}\n";
            info += $"精准度: {_flipper.Precision:P0}\n";
        }
        info += "---\n";

        foreach (var coin in coins)
        {
            string state = coin.IsSettled ? "静止" : "运动";
            string face = coin.IsFaceUp ? "正面" : "反面";
            string standing = coin.IsStanding ? " [立起来!]" : "";
            info += $"{coin.Name}: {state} {face}{standing}";
            info += $" 翻转={coin.FlipCount}次";
            info += $" 面值={coin.FaceValue}\n";
        }

        _infoLabel.Text = info;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_flipper == null) return;
        // 快捷键调参: +/- 调力度, [/] 调精准
        if (@event is InputEventKey key && key.Pressed)
        {
            switch (key.Keycode)
            {
                case Key.Equal: // +
                    _flipper.BaseForce += ForceStep;
                    break;
                case Key.Minus: // -
                    _flipper.BaseForce = Mathf.Max(
                        1f, _flipper.BaseForce - ForceStep);
                    break;
                case Key.Bracketright: // ]
                    _flipper.Precision = Mathf.Min(
                        1f, _flipper.Precision + PrecisionStep);
                    break;
                case Key.Bracketleft: // [
                    _flipper.Precision = Mathf.Max(
                        0f, _flipper.Precision - PrecisionStep);
                    break;
            }
        }
    }
}

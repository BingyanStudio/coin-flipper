using Godot;
using System.Linq;

/// <summary>
/// 调试信息面板。实时显示硬币状态和物理参数调节。
/// 服务于 Milestone 1：物理手感调试。
/// </summary>
public partial class DebugUI : Control
{
    private const float ForceStep = 1f;
    private const float PrecisionStep = 0.1f;
    private const float MinBaseForce = 1f;

    private Label _infoLabel;
    private CoinFlipper _flipper;
    private Node _coinContainer;

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
    }

    public override void _Process(double delta)
    {
        var coins = _coinContainer?.GetChildren()
            .OfType<Coin>().ToArray() ?? System.Array.Empty<Coin>();

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
        if (@event is InputEventKey key && key.Pressed)
        {
            switch (key.Keycode)
            {
                case Key.Equal:
                    _flipper.BaseForce += ForceStep;
                    break;
                case Key.Minus:
                    _flipper.BaseForce = Mathf.Max(
                        MinBaseForce, _flipper.BaseForce - ForceStep);
                    break;
                case Key.Bracketright:
                    _flipper.Precision = Mathf.Min(
                        1f, _flipper.Precision + PrecisionStep);
                    break;
                case Key.Bracketleft:
                    _flipper.Precision = Mathf.Max(
                        0f, _flipper.Precision - PrecisionStep);
                    break;
            }
        }
    }
}

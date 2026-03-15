using Godot;

/// <summary>
/// 调试快捷键处理器。处理物理参数的键盘调节输入。
/// 与 DebugUI（纯显示）分离，避免 UI 层直接承担输入职责。
/// 由 DebugBootstrap 创建并挂载。
/// </summary>
public partial class DebugInputHandler : Node
{
    private const float ForceStep = 1f;
    private const float PrecisionStep = 0.1f;

    private CoinFlipper _flipper;

    public override void _Ready()
    {
        _flipper = GetTree().Root.FindChild(
            SceneNodes.CoinFlipper, true, false) as CoinFlipper;

        if (_flipper == null)
            GD.PrintErr($"[DebugInputHandler] 未找到节点 '{SceneNodes.CoinFlipper}'，快捷键调参不可用");
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
                    _flipper.BaseForce -= ForceStep;
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

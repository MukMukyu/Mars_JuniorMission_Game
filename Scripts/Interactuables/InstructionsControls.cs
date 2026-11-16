using Godot;
using System;

public partial class InstructionsControls : Control
{
    [Export] public Label actionText;
    // [----- POSICIONES PARA CADA ANIMACIÓN -----]
    public Vector2 HIDE_ANIM_POSITION = Vector2.Zero;
    public Vector2 SHOW_ANIM_POSITION = Vector2.Zero;
    // [---- OTROS -----]
    protected Tween tweenAnim { get; private set; }



    public void ShowInstructions()
    {
        ResetTween();

        tweenAnim.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Sine);
        tweenAnim.TweenProperty(this, "position", SHOW_ANIM_POSITION, 0.3);
        tweenAnim.Parallel().TweenProperty(this, "modulate:a", 1.0f, 0.3);
    }
    public void HideInstructions()
    {
        ResetTween();

        tweenAnim.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);
        tweenAnim.TweenProperty(this, "position", HIDE_ANIM_POSITION, 1.0);
        tweenAnim.Parallel().TweenProperty(this, "modulate:a", 0, 0.3);
    }



    private void ResetTween()
    {
        if (tweenAnim != null)
            tweenAnim.Kill();

        tweenAnim = CreateTween();
    }
}

using Godot;
using System;

public partial class Raciones : Interactuable
{
    [Export] private Label foodLeftlabel;

    // [----- POSICIONES PARA CADA ANIMACIÓN -----]
    protected override Vector2 HIDE_ANIM_POSITION { get; } = new Vector2(-23, 15);
    protected override Vector2 SHOW_ANIM_POSITION { get; } = new Vector2(-23, 3);

    // [----- TEXTO Y ANIMACIÓN DE LA ACCIÓN -----]
    protected override string ACTION_LABEL { get; } = "Eat.";
    protected override Vector2 HIDE_ANIM_DIALOG_POSITION { get; } = new Vector2(14, 0);
    protected override Vector2 SHOW_ANIM_DIALOG_POSITION { get; } = new Vector2(14, -9);

    // [------ HAMBRE MANAGER ------]
    [Export] private StatsJugadorManager hambreManager;
    private const int maxRations = 150;
    private int foodLeft = maxRations;



    protected override void _OnAnimationHide()
    {
        tweenAnim.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);
        tweenAnim.TweenProperty(PopOp, "position", HIDE_ANIM_POSITION, 1.0);
        tweenAnim.Parallel().TweenProperty(PopOp, "modulate:a", 0, 1.0);
    }
    protected override void _OnAnimationShow()
    {
        tweenAnim.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Sine);
        tweenAnim.TweenProperty(PopOp, "position", SHOW_ANIM_POSITION, 0.3);
        tweenAnim.Parallel().TweenProperty(PopOp, "modulate:a", 1.0f, 1.0);
    }




    protected override void _DoOnInteract()
    {
        if (foodLeft >= 0)
        {
            hambreManager.hambreActual += 2000;
            foodLeft -= 1;
            foodLeftlabel.Text = foodLeft.ToString();
        }
    }
}

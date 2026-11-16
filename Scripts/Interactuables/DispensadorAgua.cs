using Godot;
using System;

public partial class DispensadorAgua : Interactuable
{
    [Export] private Label waterLeftlabel;

    // [----- POSICIONES PARA CADA ANIMACIÓN -----]
    protected override Vector2 HIDE_ANIM_POSITION { get; } = new Vector2(-15, 24);
    protected override Vector2 SHOW_ANIM_POSITION { get; } = new Vector2(-15, 10);

    // [----- TEXTO Y ANIMACIÓN DE LA ACCIÓN -----]
    protected override string ACTION_LABEL { get; } = "Drink.";
    protected override Vector2 HIDE_ANIM_DIALOG_POSITION { get; } = new Vector2(12, 0);
    protected override Vector2 SHOW_ANIM_DIALOG_POSITION { get; } = new Vector2(12, -9);

    // [------ HAMBRE MANAGER ------]
    [Export] private StatsJugadorManager sedManager;
    private const int maxLiters = 200;
    private float waterLeft = maxLiters;



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
        if (waterLeft >= 0)
        {
            sedManager.sedActual += 2000;
            waterLeft -= 0.4f;
            waterLeftlabel.Text = waterLeft.ToString("F1");
        }
    }
}

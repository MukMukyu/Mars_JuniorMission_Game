using Godot;
using System;

public partial class LoadingScreen : CanvasLayer
{
    // La pantalla ya tapó todo: se puede empezar a cargar.
    [Signal]
    public delegate void LoadingScreenReadyEventHandler();

    // La pantalla ya se desvaneció: se puede liberar.
    [Signal]
    public delegate void LoadingScreenFinishedEventHandler();

    private const double FadeTime = 1.0;

    private ColorRect Fade;


    public override void _Ready()
    {
        // Por encima de cualquier otro CanvasLayer (menús, HUD...).
        Layer = 100;

        // Un CanvasLayer no tiene "modulate"; lo que se anima es el ColorRect.
        Fade = GetNode<ColorRect>("ColorRect");

        Tween TweenAnim = CreateTween();
        TweenAnim.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Sine);
        TweenAnim.TweenProperty(Fade, "modulate:a", 1.0f, FadeTime);
        TweenAnim.Finished += _OnFadeInFinished;
    }


    private void _OnFadeInFinished()
    {
        EmitSignal(SignalName.LoadingScreenReady);
    }

    private void _OnFadeOutFinished()
    {
        EmitSignal(SignalName.LoadingScreenFinished);
    }


    public void _OnProgressChange(float Progress)
    {
        // Progress va de 0.0 a 1.0; aquí se actualizaría una barra de progreso.
    }

    public void _OnLoadFinished()
    {
        Tween TweenAnim = CreateTween();
        TweenAnim.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);
        TweenAnim.TweenProperty(Fade, "modulate:a", 0.0f, FadeTime);
        TweenAnim.Finished += _OnFadeOutFinished;
    }
}

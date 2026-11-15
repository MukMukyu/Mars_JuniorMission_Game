using Godot;
using System;
using System.Collections.Generic;

public partial class MedidorEnergia : Area2D
{
    protected GameManager gameManager;

    [Export] private NinePatchRect PopOp;
    [Export] private Label consumed;
    [Export] private Label produced;

    // [----- ENUM PARA LAS ANIMACIONES -----]
    public enum WindowsAnimations { HIDE, SHOW, SHOW_MORE };

    // [----- POSICIONES PARA CADA ANIMACIÓN -----]
    protected Vector2 HIDE_ANIM_POSITION = new Vector2(98.5f, 190);
    protected Vector2 SHOW_ANIM_POSITION = new Vector2(98.5f, 174);
    protected Vector2 SHOW_MORE_ANIM_POSITION = new Vector2(98.5f, 79);

    // [----- ESTADOS AL ABRIR Y CERRAR -----]
    private bool onRange = false;
    // [---- OTROS -----]
    protected Tween tweenAnim { get; private set; }



    public override void _Ready()
    {
        gameManager = GetNode<GameManager>(GetTree().CurrentScene.GetPath());

        gameManager.OnMinutePassed += _OnMinutePassed;
    }



    private void _OnMinutePassed()
    {
        produced.Text = (EnergiaManager.ProduccionActualkWh / GameManager.MarsHours_PerSecond).ToString("F2");
        consumed.Text = (EnergiaManager.ConsumoActualkWh / GameManager.MarsHours_PerSecond).ToString("F2");
    }


    private void _OnBodyEntered(Node2D body)
    {
        PlayAnimation((int)(WindowsAnimations.SHOW));
    }
    private void _OnBodyExited(Node2D body) 
    {
        PlayAnimation((int)(WindowsAnimations.HIDE));
    }



    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionReleased("Interact") && onRange)
            PlayAnimation((int)(WindowsAnimations.SHOW_MORE));
    }



    public void PlayAnimation(int animation)
    {
        ResetTween();

        switch (animation)
        {
            case 0:    // Animación "hide"
                onRange = false;
                tweenAnim.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Bounce);
                tweenAnim.TweenProperty(PopOp, "position", HIDE_ANIM_POSITION, 1.0);
                break;
            case 1:     // Animación "show"
                onRange = true;
                tweenAnim.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Bounce);
                tweenAnim.TweenProperty(PopOp, "position", SHOW_ANIM_POSITION, 0.3);
                break;
            case 2:     // Animación "show_more"
                tweenAnim.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
                tweenAnim.TweenProperty(PopOp, "position", SHOW_MORE_ANIM_POSITION, 0.5);
                break;
        }
    }


    protected void ResetTween()
    {
        if (tweenAnim != null)
            tweenAnim.Kill();

        tweenAnim = CreateTween();
    }
}

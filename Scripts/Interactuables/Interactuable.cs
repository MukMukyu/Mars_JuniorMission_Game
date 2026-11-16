using System;
using Godot;

public abstract partial class Interactuable : Area2D
{
    // [------ NODOS ------]
    protected GameManager gameManager;
    [Export] protected NinePatchRect PopOp;

    // [----- INTERACCIÓN ------]
    [Export] protected InstructionsControls labelAccion;

    // [----- ENUM PARA LAS ANIMACIONES -----]
    public enum WindowsAnimations { HIDE, SHOW, SHOW_MORE };

    // [----- POSICIONES PARA CADA ANIMACIÓN -----]
    protected abstract Vector2 HIDE_ANIM_POSITION { get; }
    protected abstract Vector2 SHOW_ANIM_POSITION { get; }
    protected virtual Vector2 SHOW_MORE_ANIM_POSITION { get; } = Vector2.Zero;

    // [----- TEXTO Y ANIMACIÓN DE LA ACCIÓN -----]
    protected abstract string ACTION_LABEL { get; }
    protected abstract Vector2 HIDE_ANIM_DIALOG_POSITION { get; }
    protected abstract Vector2 SHOW_ANIM_DIALOG_POSITION { get; }

    // [----- ESTADOS AL ABRIR Y CERRAR -----]
    private bool onRange = false;
    // [---- OTROS -----]
    protected Tween tweenAnim { get; private set; }




    public override void _Ready()
    {
        gameManager = GetNode<GameManager>(GetTree().CurrentScene.GetPath());

        gameManager.OnMinutePassed += _OnMinutePassed;

        labelAccion.actionText.Text = ACTION_LABEL;
        labelAccion.SHOW_ANIM_POSITION = SHOW_ANIM_DIALOG_POSITION;
        labelAccion.HIDE_ANIM_POSITION = HIDE_ANIM_DIALOG_POSITION;
    }
    public override void _ExitTree()
    {
        if (gameManager != null)
            gameManager.OnMinutePassed -= _OnMinutePassed;

        _DoOnExitTree();
    }
    protected virtual void _DoOnExitTree() { }




    private void _OnBodyEntered(Node2D body)
    {
        PlayAnimation((int)(WindowsAnimations.SHOW));
        _DoOnBodyEntered();
        labelAccion.ShowInstructions();
    }
    protected virtual void _DoOnBodyEntered() {}
    private void _OnBodyExited(Node2D body)
    {
        PlayAnimation((int)(WindowsAnimations.HIDE));
        _DoOnBodyExited();
        labelAccion.HideInstructions();
    }
    protected virtual void _DoOnBodyExited() {}



    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionReleased("Interact") && onRange)
            _DoOnInteract();
    }
    protected abstract void _DoOnInteract();



    protected void PlayAnimation(int animation)
    {
        ResetTween();

        switch (animation)
        {
            case 0:    // Animación "hide"
                onRange = false;
                _OnAnimationHide();
                break;
            case 1:     // Animación "show"
                onRange = true;
                _OnAnimationShow();
                break;
            case 2:     // Animación "show_more"
                _OnAnimationShowMore();
                break;
        }
    }



    protected virtual void _OnAnimationHide()
    {
        tweenAnim.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Bounce);
        tweenAnim.TweenProperty(PopOp, "position", HIDE_ANIM_POSITION, 1.0);
    }
    protected virtual void _OnAnimationShow()
    {
        tweenAnim.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Bounce);
        tweenAnim.TweenProperty(PopOp, "position", SHOW_ANIM_POSITION, 0.3);
    }
    protected virtual void _OnAnimationShowMore()
    {
        tweenAnim.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
        tweenAnim.TweenProperty(PopOp, "position", SHOW_MORE_ANIM_POSITION, 0.5);
    }



    protected void ResetTween()
    {
        if (tweenAnim != null)
            tweenAnim.Kill();

        tweenAnim = CreateTween();
    }


    protected virtual void _OnMinutePassed() { }
}

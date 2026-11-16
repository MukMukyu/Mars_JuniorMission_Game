using Godot;
using System;

public partial class PuertaExterior : StaticBody2D
{
    [Export] private Area2D area;
    [Export] private CollisionShape2D hitbox;
    [Export] private NinePatchRect door;

    // Marcar solo en las puertas que dan a Marte (Puerta Exterior y Puerta Exterior2), no en las de dentro
    // de la esclusa. El lado de fuera es el -X local: las puertas de la derecha están volteadas (scale.x = -1).
    [Export] public bool DaAlExterior = false;

    Tween tweenAnim;
    private GameManager gameManager;



    public override void _Ready()
    {
        gameManager = GetNode<GameManager>(GetTree().CurrentScene.GetPath());
    }


    private void _OnBodyEnter(Node2D body)
    {
        ResetTween();

        hitbox.SetDeferred("disabled", true);
        tweenAnim.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Expo);
        tweenAnim.TweenProperty(door, "size", new Vector2(8, 6), 0.2);
    }
    private void _OnBodyExited(Node2D body)
    {
        ResetTween();

        hitbox.SetDeferred("disabled", false);
        tweenAnim.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Expo);
        tweenAnim.TweenProperty(door, "size", new Vector2(8, 20), 0.2);

        // Al terminar de cruzar, según el lado en el que quedó el jugador, está dentro o fuera de la base.
        if (DaAlExterior && body is mainChar)
            gameManager.JugadorEnBase = ToLocal(body.GlobalPosition).X >= 0;
    }

    protected void ResetTween()
    {
        if (tweenAnim != null)
            tweenAnim.Kill();

        tweenAnim = CreateTween();
    }
}

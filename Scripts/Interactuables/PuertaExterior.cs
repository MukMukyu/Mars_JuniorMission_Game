using Godot;
using System;

public partial class PuertaExterior : StaticBody2D
{
    [Export] private Area2D area;
    [Export] private CollisionShape2D hitbox;
    [Export] private NinePatchRect door;

    Tween tweenAnim;

    
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
    }

    protected void ResetTween()
    {
        if (tweenAnim != null)
            tweenAnim.Kill();

        tweenAnim = CreateTween();
    }
}

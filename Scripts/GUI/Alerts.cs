using Godot;
using System;

public partial class Alerts : NinePatchRect
{
    [Export] private Timer timer;
    [Export] private Label titleLabel;
    [Export] private Label messageLabel;
    [Export] public string title;
    [Export] public string message;

    // [----- ENUM PARA LAS ANIMACIONES -----]
    public enum WindowsAnimations { HIDE, SHOW };

    // [----- POSICIONES PARA CADA ANIMACIÓN -----]
    protected Vector2 HIDE_ANIM_POSITION = Vector2.Zero;
    protected Vector2 SHOW_ANIM_POSITION = new Vector2(-121, 0);

    // [----- ESTADOS AL ABRIR Y CERRAR -----]
    private bool onRange = false;
    private string currectAnimation;
    // [---- OTROS -----]
    private Tween tweenAnim { get; set; }




    public static Alerts NewAlert(string _title, string _message)
    {
        PackedScene AlertScene = GD.Load<PackedScene>("uid://csjrgsrdhemo1");
        Alerts newAlert = AlertScene.Instantiate<Alerts>();

        newAlert.title = _title;
        newAlert.message = _message;

        return newAlert;
    }




    public override void _Ready()
    {
        OffsetTransformEnabled = true;

        PlayAnimation((int)WindowsAnimations.SHOW);

        tweenAnim.Finished += _OnAnimFinished;
    }



    private void Timeout()
    {
        PlayAnimation((int)WindowsAnimations.HIDE);
    }

    private void _OnAnimFinished()
    {
        if (currectAnimation == "Hide")
            QueueFree();
    }



    public void PlayAnimation(int animation)
    {
        ResetTween();

        switch (animation)
        {
            case 0:    // Animación "hide"
                currectAnimation = "Hide";
                onRange = false;
                tweenAnim.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quad);
                tweenAnim.TweenProperty(this, "offset_transform_position", HIDE_ANIM_POSITION, 0.5);
                break;
            case 1:     // Animación "show"
                currectAnimation = "Show";
                onRange = true;
                tweenAnim.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Bounce);
                tweenAnim.TweenProperty(this, "offset_transform_position", SHOW_ANIM_POSITION, 0.3);
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

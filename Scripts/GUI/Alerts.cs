using Godot;
using System;
using System.Threading.Tasks;

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
    // [---- OTROS -----]
    protected Tween tweenAnim { get; private set; }



    public override void _Ready()
    {
        OffsetTransformEnabled = true;

        PlayAnimation((int)WindowsAnimations.SHOW);
    }



    private void Timeout()
    {
        PlayAnimation((int)WindowsAnimations.HIDE);
    }



    public void PlayAnimation(int animation)
    {
        ResetTween();

        switch (animation)
        {
            case 0:    // Animación "hide"
                onRange = false;
                tweenAnim.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quad);
                tweenAnim.TweenProperty(this, "offset_transform_position", HIDE_ANIM_POSITION, 0.5);
                break;
            case 1:     // Animación "show"
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

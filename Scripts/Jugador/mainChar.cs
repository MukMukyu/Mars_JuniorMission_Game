
using Godot;
using System;

public partial class mainChar : CharacterBody2D
{
    private float velocidad = 50.0f;
    private float velocidadCorrer = 80.0f;
    private float salto = -30.60f;

    public bool mover = false;
    public bool correr = false;

    // [----- SPRITE Y ANIMACIÓN AL CAMINAR -----]
    [Export] private Sprite2D sprite;                 // el dibujo original mira a la derecha
    [Export] private float alturaSaltito = 2f;        // píxeles que sube en cada paso
    [Export] private float duracionPaso = 0.24f;      // segundos por saltito caminando (corriendo, un 30 % menos)

    private Vector2 posicionBaseSprite;
    private Tween tweenPasos;
    private bool animandoPasos = false;
    private bool animandoCorriendo = false;



    public override void _Ready()
    {
        // Se aplasta y estira desde los pies, no desde el centro: el origen del sprite pasa a su borde inferior
        // (se ve en el mismo sitio, solo cambia el punto de apoyo).
        float mitadAlto = sprite.GetRect().Size.Y / 2f;
        sprite.Offset += new Vector2(0, -mitadAlto);
        sprite.Position += new Vector2(0, mitadAlto);
        posicionBaseSprite = sprite.Position;
    }



    public override void _PhysicsProcess(double delta)
    {
 
        if (!IsOnFloor())
        {
            Vector2 vel = Velocity;
            vel.Y += ConstantesGlobales.Gravedad * (float)delta;
            Velocity = vel;
        }


        if (Input.IsActionJustPressed("Saltar") && IsOnFloor())
        {
            Vector2 vel = Velocity;
            vel.Y = salto;
            Velocity = vel;
        }


        float mov = Input.GetAxis("Izq", "Der");


        mover = mov != 0;
        correr = mover && Input.IsActionPressed("Correr");


        float velocidadActual = correr ? velocidadCorrer : velocidad;

        Vector2 movVel = Velocity;
        movVel.X = mov * velocidadActual;
        Velocity = movVel;

        MoveAndSlide();

        // Mira hacia donde camina; al pararse conserva la última dirección.
        if (mov < 0)
            sprite.FlipH = true;
        else if (mov > 0)
            sprite.FlipH = false;

        // En el aire no da saltitos: el salto ya es el movimiento.
        ActualizarSaltitos(mover && IsOnFloor());
    }



    // Saltito "de caricatura" en bucle mientras camina: se estira al subir y se aplasta al tocar el suelo.
    private void ActualizarSaltitos(bool caminando)
    {
        if (caminando == animandoPasos && correr == animandoCorriendo)
            return;

        animandoPasos = caminando;
        animandoCorriendo = correr;

        if (tweenPasos != null)
            tweenPasos.Kill();

        tweenPasos = CreateTween();

        if (!caminando)
        {
            // Vuelve a la pose normal con un pequeño rebote.
            tweenPasos.SetParallel(true).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
            tweenPasos.TweenProperty(sprite, "position", posicionBaseSprite, 0.12);
            tweenPasos.TweenProperty(sprite, "scale", Vector2.One, 0.12);
            return;
        }

        double mitadPaso = (correr ? duracionPaso * 0.7f : duracionPaso) / 2.0;
        Vector2 arriba = posicionBaseSprite + new Vector2(0, -alturaSaltito);

        tweenPasos.SetLoops();

        // Sube estirándose (frena arriba)...
        tweenPasos.TweenProperty(sprite, "position", arriba, mitadPaso)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
        tweenPasos.Parallel().TweenProperty(sprite, "scale", new Vector2(0.85f, 1.15f), mitadPaso)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);

        // ...y cae acelerando hasta aplastarse contra el suelo.
        tweenPasos.TweenProperty(sprite, "position", posicionBaseSprite, mitadPaso)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quad);
        tweenPasos.Parallel().TweenProperty(sprite, "scale", new Vector2(1.2f, 0.8f), mitadPaso)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quad);
    }
}

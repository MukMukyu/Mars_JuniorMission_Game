
using Godot;
using System;

public partial class mainChar : CharacterBody2D
{
    private float velocidad = 50.0f;
    private float velocidadCorrer = 80.0f;
    private float salto = -30.60f;

    public bool mover = false;
    public bool correr = false;

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
    }
}

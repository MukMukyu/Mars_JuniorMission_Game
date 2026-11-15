
using Godot;
using System;

public partial class mainChar : CharacterBody2D
{
    private float velocidad = 100.0f;
    private float velocidadCorrer = 200.0f;
    private float salto = -176.0f;
    private float gravedad = 371.0f;

    public bool mover = false;
    public bool correr = false;

    public override void _PhysicsProcess(double delta)
    {
 
        if (!IsOnFloor())
        {
            Vector2 vel = Velocity;
            vel.Y += gravedad * (float)delta;
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

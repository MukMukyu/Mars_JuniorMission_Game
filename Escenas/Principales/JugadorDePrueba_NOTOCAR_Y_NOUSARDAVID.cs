using Godot;
using System;

public partial class JugadorDePrueba_NOTOCAR_Y_NOUSARDAVID : CharacterBody2D
{
    [Export]
    private int Vel = 400;


    public override void _PhysicsProcess(double delta)
    {
        GetInput();
        MoveAndSlide();
    }

    private void GetInput()
    {
        Vector2 InputDir = Input.GetVector("Izq", "Der", "Arriba", "Abajo");
        Velocity = InputDir * Vel;
    }
}

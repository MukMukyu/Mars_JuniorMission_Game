using Godot;
using System;

public partial class PanelSolar : Area2D
{
    protected GameManager gameManager;
    [Export] public float KiloWattsProducidos;
    [Export] public int PorcentajeDePolvo;
    [Export] public bool Funciona { get; set; } = true;



    public override void _Ready()
    {
        gameManager = GetNode<GameManager>(GetTree().CurrentScene.GetPath());

        gameManager.OnSecondPassed += _OnSecondPassed;
    }



    public override sealed void _PhysicsProcess(double delta)
    {
        //EnergiaManager.CargaActualkWh;
    }

    private void _OnSecondPassed()
    {
        EnergiaManager.CargaActualkWh += KiloWattsProducidos;
    }
}

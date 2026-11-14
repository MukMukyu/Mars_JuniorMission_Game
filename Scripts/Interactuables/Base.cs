using Godot;
using System;

public partial class Base : Node2D
{
    protected GameManager gameManager;
    [Export] public float KiloWattsGastados = 10f;


    public override void _Ready()
    {
        gameManager = GetNode<GameManager>(GetTree().CurrentScene.GetPath());

        gameManager.OnSecondPassed += _OnSecondPassed;
    }



    public override void _ExitTree()
    {
        if (gameManager != null)
            gameManager.OnSecondPassed -= _OnSecondPassed;
    }



    private void _OnSecondPassed()
    {
        EnergiaManager.CargaActualkWh -= KiloWattsGastados * (float)GameManager.MarsHours_PerSecond;
    }
}

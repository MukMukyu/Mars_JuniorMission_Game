using Godot;
using System;

public partial class Base : Node2D
{
    protected GameManager gameManager;
    [Export] public float KiloWattsGastados = 10f;
    private float kiloWattsConsumidosActuales = 0;
    public float KiloWattsConsumidosActuales
    {
        get => kiloWattsConsumidosActuales;
        set
        {
            if (value == kiloWattsConsumidosActuales) return;

            EnergiaManager.ConsumoActualkWh -= kiloWattsConsumidosActuales * (float)GameManager.MarsHours_PerSecond;
            EnergiaManager.ConsumoActualkWh += value * (float)GameManager.MarsHours_PerSecond;
            kiloWattsConsumidosActuales = value;
        }
    }


    public override void _Ready()
    {
        gameManager = GetNode<GameManager>(GetTree().CurrentScene.GetPath());

        gameManager.OnMinutePassed += _OnMinutePassed;
    }



    public override void _ExitTree()
    {
        if (gameManager != null)
            gameManager.OnMinutePassed -= _OnMinutePassed;
    }



    private void _OnMinutePassed()
    {
        KiloWattsConsumidosActuales = KiloWattsGastados;
    }
}

using Godot;
using System;

public partial class EnergiaManager : MainGUI
{
    // [----- BATERÍA Y OTROS -----]
    private const float CapacidadTotalkWh = 200;
    private static float cargaActualkWh = CapacidadTotalkWh * 0.70f;
    public static float CargaActualkWh
    {
        get => cargaActualkWh;
        set => cargaActualkWh = Mathf.Clamp(value, 0, CapacidadTotalkWh);
    }

    // [----- BARRA ENERGÍA -----]
    [Export] private ProgressBar BarraEnergia;



    public override void _Ready()
    {
        base._Ready();

        BarraEnergia.MaxValue = CapacidadTotalkWh;
        BarraEnergia.Value = CargaActualkWh;
    }

    protected override void _OnSecondPassed()
    {
        if (gameManager.SecondsCounter % 1 == 0)
        {
            if (CargaActualkWh <= 0)
                return;

            BarraEnergia.Value = CargaActualkWh;

            //GD.Print("Oxígeno: " + oxigenoActual);
        }
    }
}

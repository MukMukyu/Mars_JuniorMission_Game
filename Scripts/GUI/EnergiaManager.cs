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
    [Export] private TextureProgressBar BarraEnergia;
    [Export] private Label Porcentaje;



    public override void _Ready()
    {
        base._Ready();

        BarraEnergia.MaxValue = CapacidadTotalkWh;
        BarraEnergia.Value = CargaActualkWh;
        Porcentaje.Text = Mathf.Floor((CargaActualkWh / CapacidadTotalkWh) * 100).ToString("F0") + "%";
    }

    protected override void _OnSecondPassed()
    {
        if (CargaActualkWh <= 0)
            return;

        BarraEnergia.Value = CargaActualkWh;
        Porcentaje.Text = Mathf.Floor((CargaActualkWh/CapacidadTotalkWh) * 100).ToString("F0") + "%";

            //GD.Print("Oxígeno: " + oxigenoActual);
    }
}

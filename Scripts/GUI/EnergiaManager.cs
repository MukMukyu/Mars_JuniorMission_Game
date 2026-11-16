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
    private static float consumoActualkWh = 0;
    public static float ConsumoActualkWh
    {
        get => consumoActualkWh;
        set
        {
            consumoActualkWh = Mathf.Clamp(value, 0, float.MaxValue);
        }
    }
    private static float produccionActualkWh = 0;
    public static float ProduccionActualkWh
    {
        get => produccionActualkWh;
        set
        {
            produccionActualkWh = Mathf.Clamp(value, 0, float.MaxValue);
        }
    }

    // [----- BARRA ENERGÍA -----]
    [Export] private TextureProgressBar BarraEnergia;
    [Export] private Label Porcentaje;



    public override void _Ready()
    {
        base._Ready();

        // Estos valores son estáticos y sobreviven al cambio de escena (RESTART del Game Over):
        // hay que volver al estado inicial o la partida nueva empezaría con la batería de la anterior.
        CargaActualkWh = CapacidadTotalkWh * 0.70f;
        ConsumoActualkWh = 0;
        ProduccionActualkWh = 0;

        BarraEnergia.MaxValue = CapacidadTotalkWh;
        BarraEnergia.Value = CargaActualkWh;
        Porcentaje.Text = Mathf.Floor((CargaActualkWh / CapacidadTotalkWh) * 100).ToString("F0") + "%";
    }

    protected override void _OnMinutePassed()
    {
        // Sin "return" con batería a 0: si no, al amanecer los paneles nunca podrían recargarla.
        CargaActualkWh += ProduccionActualkWh - ConsumoActualkWh;

        BarraEnergia.Value = CargaActualkWh;
        Porcentaje.Text = Mathf.Floor((CargaActualkWh/CapacidadTotalkWh) * 100).ToString("F0") + "%";

            //GD.Print("Oxígeno: " + oxigenoActual);
    }
}

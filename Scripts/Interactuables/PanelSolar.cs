using Godot;
using System;

public partial class PanelSolar : Area2D
{
    protected GameManager gameManager;
    // Potencia del panel (kW): cuánta energía da por HORA, no por tick.
    [Export] public float KiloWattsProducidos = 20f;

    [Export] public int PorcentajeDePolvo;
    [Export] public bool Funciona { get; set; } = true;

    // Potencia real en este momento: 0 si está roto, y baja según el polvo acumulado.
    public float PotenciaActualkW
    {
        get
        {
            if (!Funciona)
                return 0f;

            float limpieza = 1f - Mathf.Clamp(PorcentajeDePolvo, 0, 100) / 100f;
            return KiloWattsProducidos * limpieza;
        }
    }



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
        // Energía (kWh) = potencia (kW) x tiempo (h) transcurrido en este tick.
        EnergiaManager.CargaActualkWh += PotenciaActualkW * (float)GameManager.MarsHours_PerSecond;
    }
}

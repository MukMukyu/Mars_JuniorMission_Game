using Godot;
using System;

public partial class PanelSolar : Area2D
{
    protected GameManager gameManager;
    // Potencia del panel (kW): cuánta energía da por HORA, no por tick.
    [Export] public float KiloWattsProducidos = 20f;

    [Export] public float PorcentajeDePolvo;
    [Export] public bool Funciona { get; set; } = true;

    private RandomNumberGenerator rng;
    private float randomSeed;


    private float PotencialPorHora = 0;
    // Potencia real en este momento: 0 si está roto, y baja según el polvo acumulado.
    public float PotenciaActualkW
    {
        get
        {
            if (!Funciona)
                return 0f;

            float limpieza = 1f - Mathf.Clamp(PorcentajeDePolvo, 0, 100) / 100f;
            return KiloWattsProducidos * limpieza * PotencialPorHora;
        }
    }



    public override void _Ready()
    {
        gameManager = GetNode<GameManager>(GetTree().CurrentScene.GetPath());

        gameManager.OnSecondPassed += _OnSecondPassed;

        rng = new RandomNumberGenerator();
    }



    public override void _ExitTree()
    {
        if (gameManager != null)
            gameManager.OnSecondPassed -= _OnSecondPassed;
    }


    private void _OnSecondPassed()
    {
        PotencialPorHora = CalcularPotencialkW();

        // Energía (kWh) = potencia (kW) x tiempo (h) transcurrido en este tick.
        EnergiaManager.CargaActualkWh += PotenciaActualkW * (float)GameManager.MarsHours_PerSecond;

        Empolvarse();

        Romperse();
    }

    private float CalcularPotencialkW()
    {
        double anguloPotencia = Mathf.Remap(gameManager.MinutoMarciano, 0, 86400, 0, 360);
        return (float)Math.Sin(anguloPotencia);
    }
    private void Romperse()
    {
        randomSeed = rng.RandfRange(0f, 10000f);

        if (randomSeed < 1 * ConstantesGlobales.DCI && Funciona)
        {
            GD.Print("Se ha roto el" + Name);
            Funciona = false;
        }
    }

    private void Empolvarse()
    {
        int multiplicadorPolvo = gameManager.IsDustStorm ? 100 : 10;
        PorcentajeDePolvo += (float)(multiplicadorPolvo * ConstantesGlobales.DCI);
    }
}

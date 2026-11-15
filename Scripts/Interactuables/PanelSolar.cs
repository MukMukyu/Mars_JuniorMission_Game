using Godot;
using System;

public partial class PanelSolar : Area2D
{
    protected GameManager gameManager;
    // Potencia del panel (kW): cuánta energía da por HORA, no por tick.
    [Export] public float KiloWattsProducidos = 40f;

    [Export] public float PorcentajeDePolvo;
    [Export] public bool Funciona { get; set; } = true;

    private RandomNumberGenerator rng;
    private float randomSeed;


    // Potencia real en este momento: 0 si está roto, y baja según el polvo acumulado.
    private float potenciaActualkW = 0;
    public float PotenciaActualkW
    {
        get
        {
            if (!Funciona)
                return 0f;

            return potenciaActualkW;
        }
        set
        {
            if (value == potenciaActualkW) return;

            EnergiaManager.ProduccionActualkWh -= potenciaActualkW * (float)GameManager.MarsHours_PerSecond;
            EnergiaManager.ProduccionActualkWh += value * (float)GameManager.MarsHours_PerSecond;
            potenciaActualkW = value;
        }
    }



    public override void _Ready()
    {
        gameManager = GetNode<GameManager>(GetTree().CurrentScene.GetPath());

        gameManager.OnMinutePassed += _OnMinutePassed;

        rng = new RandomNumberGenerator();
    }



    public override void _ExitTree()
    {
        if (gameManager != null)
            gameManager.OnMinutePassed -= _OnMinutePassed;
    }


    private void _OnMinutePassed()
    {
        float limpieza = 1f - Mathf.Clamp(PorcentajeDePolvo, 0, 100) / 100f;

        PotenciaActualkW = CalcularPotencialkW() * KiloWattsProducidos * limpieza;

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

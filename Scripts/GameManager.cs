
using Godot;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

public partial class GameManager : Node
{
    [Export] private Timer TimerDia;

    // [----- CONSTANTS -----]
    public const double MarsHours_PerSecond = 24 / 86400;

    // [----- ACTIONS & SIGNALS -----]
    public event Action OnSecondPassed;
    public event Action OnDayPassed;
    public event Action OnDustStorm;

    // [----- OTHERS -----]
    public bool IsDustStorm = false;
    private double elapsedTime = 0.0;

    private double minutoMarciano = 0;
    public double MinutoMarciano
    {
        get => minutoMarciano;
        set
        {
            if (value >= 60)
            {
                HoraMarciana += 1;
                TormentaDeArena();
                minutoMarciano = 0;
            }
            else
            {
                minutoMarciano = value;
            }
        }
    }
    private double horaMarciana = 0;
    public double HoraMarciana
    {
        get => horaMarciana;
        set
        {
            if (value >= 24)
                horaMarciana = 0;
            else
                horaMarciana = value;
        }
    }
    


    private void TormentaDeArena()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();

        float randomSeed = rng.RandfRange(0f, 1000f);

        if (randomSeed < 1 * ConstantesGlobales.DCI && IsDustStorm)
        {
            GD.Print("Tormenta de Arena");
            IsDustStorm = true;
            OnDustStorm?.Invoke();
        }
    }



    private void Reloj()
    {
        elapsedTime += TimerDia.WaitTime;
        MinutoMarciano += 1;

        GD.Print(MinutoMarciano);

        OnSecondPassed?.Invoke();

        if (elapsedTime >= 144)
        {
            elapsedTime = 0.0;
            //GD.Print(Minuo}}}}Marciano);

            OnDayPassed?.Invoke();
        }
    }
}

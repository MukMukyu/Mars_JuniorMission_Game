
using Godot;
using System;

public partial class GameManager : Node
{
    [Export] private Timer TimerDia;

    // [----- CONSTANTS -----]

    private const double RealMarsDay_Hours = 24.6;
    private const double RealMarsDay_Seconds = 60 * 60 * RealMarsDay_Hours;
    public const double MarsDay_InHours = RealMarsDay_Hours / 1000;
    public const double MarsDay_InSeconds = RealMarsDay_Seconds / 1000;

    // Horas de juego que pasan en cada tick de OnSecondPassed (1 s real = 1/36 h de Marte).
    public const double MarsHours_PerSecond = MarsDay_InHours / MarsDay_InSeconds;

    // [----- ACTIONS & SIGNALS -----]

    public event Action OnSecondPassed;
    public event Action OnDayPassed;

    // [----- OTHERS -----]

    public int SecondsCounter { get; private set; } = 0;

    private double elapsedTime = 0.0;


    private double segundoMarciano = 0;
    public double SegundoMarciano
    {
        get => segundoMarciano;
        set
        {
            if (value >= 60)
            {
                MinutoMarciano += 1;
                segundoMarciano = 0;
            }
            else
            {
                segundoMarciano = value;
            }
        }
    }
    private double minutoMarciano = 0;
    public double MinutoMarciano
    {
        get => minutoMarciano;
        set
        {
            if (value >= 60)
            {
                HoraMarciana += 1;
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
    



    private void Reloj()
    {

        elapsedTime += TimerDia.WaitTime;
        SegundoMarciano += 2;

        int newSec = (int)Math.Floor(elapsedTime * 10);


        if (newSec > SecondsCounter)
        {
            SecondsCounter = newSec;
            //GD.Print(SecondsCounter);
            OnSecondPassed?.Invoke();
        }

        if (elapsedTime >= MarsDay_InSeconds)
        {
            elapsedTime = 0.0;
            SecondsCounter = 0;
            //GD.Print(SecondsCounter);

            OnDayPassed?.Invoke();
        }
    }
}

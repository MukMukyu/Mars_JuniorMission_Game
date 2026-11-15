
using Godot;
using System;

public partial class GameManager : Node
{
    [Export] private Timer TimerDia;

    // [----- CONSTANTS -----]

    private const double RealMarsDay_Hours = 24.6;
    private const double RealMarsDay_Seconds = 60 * 60 * RealMarsDay_Hours;
    public const double MarsDay_InHours = RealMarsDay_Hours / 100;
    public const double MarsDay_InSeconds = RealMarsDay_Seconds / 100;

    // Horas de juego que pasan en cada tick de OnSecondPassed (1 s real = 1/36 h de Marte).
    public const double MarsHours_PerSecond = MarsDay_InHours / MarsDay_InSeconds;

    // [----- ACTIONS & SIGNALS -----]

    public event Action OnSecondPassed;
    public event Action OnDayPassed;

    // [----- OTHERS -----]

    public int SecondsCounter { get; private set; } = 0;

    private double elapsedTime = 0.0;

    // Hora local marciana del sol actual, de 0 a 24 (un sol se divide a 0.4 "horas marcianas" por los minutos en un día).
    public double HoraMarciana => elapsedTime / MarsDay_InSeconds * 24;



    private void Reloj()
    {

        elapsedTime += TimerDia.WaitTime;

        int newSec = (int)Math.Floor(elapsedTime);


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

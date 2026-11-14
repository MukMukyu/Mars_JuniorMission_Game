
using Godot;
using System;

public partial class GameManager : Node
{
    [Export] private Timer TimerDia;
    
    // [----- CONSTANTS -----]

    public const double MarsDay_InSeconds = 885.6;
    public const double MarsDay_InHours = 24.6;

    // Horas de juego que pasan en cada tick de OnSecondPassed (1 s real = 1/36 h de Marte).
    public const double MarsHours_PerSecond = MarsDay_InHours / MarsDay_InSeconds;

    // [----- ACTIONS & SIGNALS -----]

    public event Action OnSecondPassed;
    public event Action OnDayPassed;

    // [----- OTHERS -----]

    public int SecondsCounter { get; private set; } = 0;

    private double elapsedTime = 0.0;



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

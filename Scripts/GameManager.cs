using Godot;
using System;

public partial class GameManager : Node
{
    [Export]
    private Timer TimerDia;
    [Export]
    private Control Energia;
    [Export]
    private Control Oxigeno;
    [Export]
    private Control StatsPersonaje;

    // [----- CONSTANTS -----]
    // How many seconds does a day in mars have reduced (88560sec/100) for game purposes
    public const double MarsDay_Seconds = 885.6;

    // [----- ACTIONS & SIGNALS -----]
    public event Action OnSecondPassed;
    public event Action OnDayPassed;

    // [----- OTHERS -----]
    private double secondsCounter = 0.0;
    public double SecondsCounter 
    {
        get => secondsCounter;
        set
        {
            if (value == MarsDay_Seconds)       // When a day pass
            {
                OnDayPassed?.Invoke();
                secondsCounter = 0.0f;
            }
            else if ((value % 1.0) == 0.0)      // When a second pass
            {
                OnSecondPassed?.Invoke();
                secondsCounter = value;
            }
            else { secondsCounter = value; }    // Other
        } 
    }

    private void Reloj()
    {
        SecondsCounter += TimerDia.WaitTime;
    }
}

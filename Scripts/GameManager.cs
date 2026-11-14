
using Godot;
using System;

public partial class GameManager : Node
{
    [Export] private Timer TimerDia;
    [Export] private HoraManager HoraManager;
    [Export] private EnergiaManager EnergiaManager;
    [Export] private OxigenoManager OxigenoManager;
    [Export] private StatsJugadorManager StatsPersonajeManager;

    // [----- CONSTANTS -----]

    public const double MarsDay_Seconds = 885.6;

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
            OnSecondPassed?.Invoke();
        }

        if (elapsedTime >= MarsDay_Seconds)
        {
            elapsedTime = 0.0;
            SecondsCounter = 0;

            OnDayPassed?.Invoke();
        }
    }
}

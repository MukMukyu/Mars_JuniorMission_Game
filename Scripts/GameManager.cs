using Godot;
using System;

public partial class GameManager : Node
{
    [Export]
    private Timer TimerDia;
    [Export]
    private HoraManager HoraManager;
    [Export]
    private EnergiaManager EnergiaManager;
    [Export]
    private OxigenoManager OxigenoManager;
    [Export]
    private StatsJugadorManager StatsPersonajeManager;

    // [----- CONSTANTS -----]
    // How many seconds does a day in mars have reduced (88560sec/100) for game purposes
    public const float MarsDay_Seconds = 885.6f;

    // [----- ACTIONS & SIGNALS -----]
    public event Action OnSecondPassed;
    public event Action OnDayPassed;

    // [----- OTHERS -----]
    public int SecondsCounter = 0;
    private float millisecondsCounter = 0;
    public float MillisecondsCounter
    {
        get => millisecondsCounter;
        set
        {
            if (value == MarsDay_Seconds)       // When a day pass
            {
                OnDayPassed?.Invoke();
                millisecondsCounter = 0;
            }
            else if ((value % 1) >= 0 && (value % 1) < 0.1f)      // When a second pass
            {
                OnSecondPassed?.Invoke();
                millisecondsCounter = value;
            }
            else { millisecondsCounter = value; }    // Other
        } 
    }



    public void WhenPlayerDied()
    {

    }



    private void Reloj()
    {
        SecondsCounter += 0.1f;
        GD.Print(SecondsCounter);
    }
}

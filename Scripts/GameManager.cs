
using Godot;
using System;

public partial class GameManager : Node
{
    [Export] private Timer TimerDia;
    
    // [----- CONSTANTS -----]

    public const double MarsDay_InSeconds = 885.6;

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

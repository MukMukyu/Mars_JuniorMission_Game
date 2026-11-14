using Godot;
using System;

public partial class StatsJugadorManager : MainGUI
{
    [Export] private ProgressBar Hambre;
    [Export] private ProgressBar Sed;

    private const int HambreMaximo = 100;
    private const int SedMaximo = 100;
    private int hambreActual = HambreMaximo;
    private int sedActual = SedMaximo;

    public override void _Ready()
    {
        base._Ready();

        Hambre.MaxValue = HambreMaximo;
        Hambre.Value = hambreActual;

        Sed.MaxValue = SedMaximo;
        Sed.Value = sedActual;
    }

    protected override void _OnSecondPassed()
    {
        if (gameManager.SecondsCounter % 5 == 0)
        {
            if (hambreActual <= 0)
                return;
            else if (sedActual <= 0)
                return;
            

            hambreActual--;
            sedActual--;

            Hambre.Value = hambreActual;
            Sed.Value = sedActual;

        }
    }
}

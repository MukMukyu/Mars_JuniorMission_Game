
using Godot;
using System;

public partial class OxigenoManager : MainGUI
{
    [Export] private ProgressBar BarraOxigeno;

    private const int OxigenoMaximo = 100;
    private int oxigenoActual = OxigenoMaximo;

    public override void _Ready()
    {
        base._Ready();

        BarraOxigeno.MaxValue = OxigenoMaximo;
        BarraOxigeno.Value = oxigenoActual;

        _InitSignals();
    }

    protected override void _OnSecondPassed()
    {

        if (gameManager.SecondsCounter % 2 == 0)
        {
            if (oxigenoActual <= 0)
                return;

            oxigenoActual--;

            BarraOxigeno.Value = oxigenoActual;

            //GD.Print("Oxígeno: " + oxigenoActual);
        }
    }

    protected override void _OnDayPassed()
    {

    }
}

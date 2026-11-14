using Godot;
using System;

public partial class EnergiaManager : MainGUI
{
    [Export] private ProgressBar BarraEnergia;

    private const int EnergiaMaximo = 100;
    private int EnergiaActual = EnergiaMaximo;

    public override void _Ready()
    {
        BarraEnergia.MaxValue = EnergiaMaximo;
        BarraEnergia.Value = EnergiaActual;
    }

    protected override void _OnSecondPassed()
    {

        if (gameManager.SecondsCounter % 1 == 0)
        {
            if (EnergiaActual <= 0)
                return;

            EnergiaActual--;

            BarraEnergia.Value = EnergiaActual;

            //GD.Print("Oxígeno: " + oxigenoActual);
        }
    }
}

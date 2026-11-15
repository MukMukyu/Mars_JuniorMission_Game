using Godot;
using System;

public partial class StatsJugadorManager : MainGUI
{
    [Export] private ProgressBar Hambre;
    [Export] private ProgressBar Sed;
    [Export] private mainChar Jugador;

    private const int HambreMaximo = 100;
    private const int SedMaximo = 10000;
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
        int intervaloHambre = Jugador.correr ? 40 : 32;

        int consumoSed = Jugador.correr ? 2 : 1;

        if (gameManager.MinutoMarciano % intervaloHambre == 0)
        {
            if (hambreActual > 0)
            {
                hambreActual--;
                Hambre.Value = hambreActual;
            }
        }

        if (sedActual > 0)
        {
            sedActual -= consumoSed;

            if (sedActual < 0)
                sedActual = 0;

            Sed.Value = sedActual;
        }
    }
}

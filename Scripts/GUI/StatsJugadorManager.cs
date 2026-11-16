using Godot;
using System;

public partial class StatsJugadorManager : MainGUI
{
    [Export] private TextureProgressBar Hambre;
    [Export] private TextureProgressBar Sed;
    [Export] private mainChar Jugador;

    private const int HambreMaximo = 3000;
    private const int SedMaximo = 3000;
    public int hambreActual = HambreMaximo;
    public int sedActual = SedMaximo;

    public override void _Ready()
    {
        base._Ready();

        Hambre.MaxValue = HambreMaximo;
        Hambre.Value = hambreActual;

        Sed.MaxValue = SedMaximo;
        Sed.Value = sedActual;
    }

    protected override void _OnMinutePassed()
    {
        int consumoHambre = Jugador.correr ? 2 : 1;

        int consumoSed = Jugador.correr ? 3 : 2;


        if (hambreActual > 0)
        {
            hambreActual = Mathf.Clamp(hambreActual - consumoHambre, 0, HambreMaximo);
            Hambre.Value = hambreActual;
        }

        if (sedActual > 0)
        {
            sedActual = Mathf.Clamp(sedActual - consumoSed, 0, SedMaximo);
            Sed.Value = sedActual;
        }
    }
}

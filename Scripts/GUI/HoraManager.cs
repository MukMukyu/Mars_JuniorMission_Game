using Godot;
using System;

public partial class HoraManager : MainGUI
{
    [Export]
    private Label horas;
    [Export]
    private Label minutos;
    [Export]
    private Label segundos;

    private int SegundoCounter = 0;
    private int MinutoCounter = 0;
    private int HoraCounter = 0;



    protected override void _OnSecondPassed()
    {
        SegundoCounter += 1;
        if (SegundoCounter != 60) {
            segundos.Text = SegundoCounter.ToString();
    }
}

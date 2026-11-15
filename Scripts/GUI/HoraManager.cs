using Godot;
using System;

public partial class HoraManager : MainGUI
{
    [Export] private Label horasMarte;
    [Export] private Label minutosMarte;



    protected override sealed void _OnSecondPassed()
    {
        if (gameManager.MinutoMarciano < 10)
            minutosMarte.Text = "0" + gameManager.MinutoMarciano.ToString();
        else
            minutosMarte.Text = gameManager.MinutoMarciano.ToString();

        if (gameManager.HoraMarciana < 10)
            horasMarte.Text = "0" + gameManager.HoraMarciana.ToString();
        else
            horasMarte.Text = gameManager.HoraMarciana.ToString();

        //GD.Print(SegundoCounter);
    }
}

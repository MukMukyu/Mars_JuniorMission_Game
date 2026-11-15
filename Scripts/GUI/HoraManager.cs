using Godot;
using System;

public partial class HoraManager : MainGUI
{
    [Export] private Label horasTierra;
    [Export] private Label minutosTierra;
    [Export] private Label horasMarte;
    [Export] private Label minutosMarte;

    private int segundoCounter = 0;
    private int SegundoCounter
    {
        get => segundoCounter;
        set
        {
            if ((value % 60) == 0)
            {
                MinutoCounter += 1;
                segundoCounter = 0;
            }
            else
            {
                segundoCounter = value;
            }
        }
    }

    private int minutoCounter = 0;
    private int MinutoCounter
    {
        get => minutoCounter;
        set
        {
            if ((value % 60) == 0)
            {
                HoraCounter += 1;
                minutoCounter = 0;
            }
            else
            {
                minutoCounter = value;
            }
        }
    }

    private int HoraCounter { get; set; } = 0;



    protected override sealed void _OnSecondPassed()
    {
        if (gameManager.SecondsCounter % 1 == 0)
        {
            SegundoCounter++;

            horasMarte.Text = Mathf.Floor(gameManager.HoraMarciana).ToString();
            minutosMarte.Text = Mathf.Floor(gameManager.HoraMarciana % 60).ToString();

            if (MinutoCounter < 10)
                minutosTierra.Text = "0" + MinutoCounter.ToString();
            else
                minutosTierra.Text = MinutoCounter.ToString();

            if (HoraCounter < 10)
                horasTierra.Text = "0" + HoraCounter.ToString();
            else
                horasTierra.Text = MinutoCounter.ToString();
        }

        //GD.Print(SegundoCounter);
    }

    protected override sealed void _OnDayPassed()
    {
        HoraCounter = 0;
        horasTierra.Text = HoraCounter.ToString();
    }
}

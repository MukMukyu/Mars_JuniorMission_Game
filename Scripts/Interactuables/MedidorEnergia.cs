using Godot;
using System;
using System.Collections.Generic;

public partial class MedidorEnergia : Interactuable
{
    [Export] private Label consumed;
    [Export] private Label produced;

    // [----- POSICIONES PARA CADA ANIMACIÓN -----]
    protected override Vector2 HIDE_ANIM_POSITION { get; } = new Vector2(98.5f, 190);
    protected override Vector2 SHOW_ANIM_POSITION { get; } = new Vector2(98.5f, 174);
    protected override Vector2 SHOW_MORE_ANIM_POSITION { get; } = new Vector2(98.5f, 79);

    // [----- TEXTO Y ANIMACIÓN DE LA ACCIÓN -----]
    protected override string ACTION_LABEL { get; } = "Check the energy stats.";
    protected override Vector2 HIDE_ANIM_DIALOG_POSITION { get; } = new Vector2(7, 2);
    protected override Vector2 SHOW_ANIM_DIALOG_POSITION { get; } = new Vector2(7, -17);



    protected override void _OnMinutePassed()
    {
        produced.Text = (EnergiaManager.ProduccionActualkWh / GameManager.HorasPorMinutoMarciano).ToString("F2");
        consumed.Text = (EnergiaManager.ConsumoActualkWh / GameManager.HorasPorMinutoMarciano).ToString("F2");
    }


    protected override void _DoOnInteract()
    {
        PlayAnimation((int)(WindowsAnimations.SHOW_MORE));
    }
}

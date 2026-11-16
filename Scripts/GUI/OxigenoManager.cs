
using Godot;
using System;

// Tanque de oxígeno del traje.
//  - Dentro de la base se rellena y, cuando está lleno, la barra se oculta.
//  - Al salir por una puerta exterior la barra aparece y empieza a gastarse.
public partial class OxigenoManager : MainGUI
{
    [Export] private TextureProgressBar BarraOxigeno;

    // [----- TANQUE -----]
    private const int OxigenoMaximo = 100;
    [Export] private int RellenoPorMinuto = 5;     // dentro: unidades que se recuperan cada minuto marciano
    [Export] private int MinutosPorUnidad = 2;     // fuera: se gasta 1 unidad cada N minutos marcianos

    private int oxigenoActual = OxigenoMaximo;
    public int OxigenoActual => oxigenoActual;
    private int minutosFuera = 0;

    // [----- MOSTRAR / OCULTAR -----]
    private bool barraVisible;
    private Tween tweenAnim;



    public override void _Ready()
    {
        base._Ready();

        BarraOxigeno.MaxValue = OxigenoMaximo;
        BarraOxigeno.Value = oxigenoActual;

        gameManager.OnJugadorCambiaZona += _OnJugadorCambiaZona;

        // La partida empieza dentro de la base con el tanque lleno: barra oculta.
        barraVisible = !gameManager.JugadorEnBase;
        Modulate = new Color(Modulate, barraVisible ? 1f : 0f);
    }

    public override void _ExitTree()
    {
        if (gameManager != null)
            gameManager.OnJugadorCambiaZona -= _OnJugadorCambiaZona;
    }



    private void _OnJugadorCambiaZona(bool enBase)
    {
        if (!enBase)
        {
            // Sale de la base: se revela la barra y empieza a contar desde cero.
            minutosFuera = 0;
            MostrarBarra(true);
        }
    }

    protected override void _OnMinutePassed()
    {
        if (gameManager.JugadorEnBase)
        {
            oxigenoActual = Math.Min(OxigenoMaximo, oxigenoActual + RellenoPorMinuto);

            // Se deja ver mientras se rellena y se oculta al llenarse.
            if (oxigenoActual == OxigenoMaximo)
                MostrarBarra(false);
        }
        else
        {
            minutosFuera++;

            if (minutosFuera % MinutosPorUnidad == 0)
                oxigenoActual = Math.Max(0, oxigenoActual - 1);
        }

        BarraOxigeno.Value = oxigenoActual;
    }



    private void MostrarBarra(bool mostrar)
    {
        if (mostrar == barraVisible) return;
        barraVisible = mostrar;

        if (tweenAnim != null)
            tweenAnim.Kill();

        tweenAnim = CreateTween();
        tweenAnim.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
        tweenAnim.TweenProperty(this, "modulate:a", mostrar ? 1f : 0f, 0.3);
    }
}

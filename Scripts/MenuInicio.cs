using Godot;
using System;

// Menú de inicio: "Jugar" carga la partida con pantalla de carga (SceneLoader) y "Salir" cierra el juego.
public partial class MenuInicio : CanvasLayer
{
    [Export] private Button botonJugar;
    [Export] private Button botonSalir;

    public override void _Ready()
    {
        botonJugar.Pressed += OnJugarPressed;
        botonSalir.Pressed += OnSalirPressed;
    }

    private void OnJugarPressed()
    {
        SceneLoader.LoadScene(ConstantesGlobales.EscenaPartida);
    }

    private void OnSalirPressed()
    {
        GetTree().Quit();
    }
}

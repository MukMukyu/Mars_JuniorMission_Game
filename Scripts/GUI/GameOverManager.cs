using Godot;
using System;

// Revisa cada minuto marciano las condiciones de derrota. Si se cumple alguna, pausa el juego
// y muestra la pantalla de Game Over con el motivo.
//  1. Oxígeno a 0 (solo se gasta fuera de la base).
//  2. Dentro de la base por debajo de TemperaturaMinimaC más de MinutosMaximosConFrio seguidos.
//  3. Sed a 0 durante más de SolesMaximosSinAgua soles.
//  4. Hambre a 0 durante más de SolesMaximosSinComida soles.
public partial class GameOverManager : CanvasLayer
{
    [Export] private OxigenoManager oxigeno;
    [Export] private StatsJugadorManager stats;
    [Export] private Label motivoLabel;

    // [----- LÍMITES -----]
    [Export] public double TemperaturaMinimaC = -10;
    [Export] public int MinutosMaximosConFrio = 10;      // minutos marcianos seguidos
    [Export] public int SolesMaximosSinAgua = 3;
    [Export] public int SolesMaximosSinComida = 14;

    private const int MinutosPorSol = 24 * 60;

    private GameManager gameManager;
    private int minutosConFrio = 0;
    private int minutosSinAgua = 0;
    private int minutosSinComida = 0;
    private bool terminado = false;



    public override void _Ready()
    {
        gameManager = GetNode<GameManager>(GetTree().CurrentScene.GetPath());
        gameManager.OnMinutePassed += _OnMinutePassed;

        Visible = false;
    }

    public override void _ExitTree()
    {
        if (gameManager != null)
            gameManager.OnMinutePassed -= _OnMinutePassed;
    }



    private void _OnMinutePassed()
    {
        if (terminado) return;

        // 1. Oxígeno.
        if (oxigeno.OxigenoActual <= 0)
        {
            GameOver("You ran out of oxygen outside the base.");
            return;
        }

        // 2. Frío. Solo cuenta dentro de la base: fuera el traje protege y lo que limita es el oxígeno.
        bool conFrio = gameManager.JugadorEnBase && TemperaturaInterna.InteriorC < TemperaturaMinimaC;
        minutosConFrio = conFrio ? minutosConFrio + 1 : 0;
        if (minutosConFrio > MinutosMaximosConFrio)
        {
            GameOver($"Hypothermia: the base stayed below {TemperaturaMinimaC:0} °C for more than {MinutosMaximosConFrio} Martian minutes.");
            return;
        }

        // 3. Sed: tiempo seguido con la barra a 0 (se reinicia al beber).
        minutosSinAgua = stats.sedActual <= 0 ? minutosSinAgua + 1 : 0;
        if (minutosSinAgua > SolesMaximosSinAgua * MinutosPorSol)
        {
            GameOver($"Dehydration: more than {SolesMaximosSinAgua} sols without water.");
            return;
        }

        // 4. Hambre: tiempo seguido con la barra a 0 (se reinicia al comer).
        minutosSinComida = stats.hambreActual <= 0 ? minutosSinComida + 1 : 0;
        if (minutosSinComida > SolesMaximosSinComida * MinutosPorSol)
        {
            GameOver($"Starvation: more than {SolesMaximosSinComida} sols without food.");
            return;
        }
    }



    private void GameOver(string motivo)
    {
        terminado = true;
        GD.Print("GAME OVER: " + motivo);

        motivoLabel.Text = motivo;
        Visible = true;
        GetTree().Paused = true;   // este nodo usa process_mode = Always, así que sus botones siguen funcionando
    }

    // El juego sigue en pausa detrás de la pantalla de carga; SceneLoader la quita al cambiar de escena.
    private void _OnReiniciarPressed()
    {
        SceneLoader.LoadScene(ConstantesGlobales.EscenaPartida);
    }

    private void _OnMenuPressed()
    {
        SceneLoader.LoadScene(ConstantesGlobales.EscenaMenu);
    }
}

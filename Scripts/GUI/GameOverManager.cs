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
    [Export] private Label sobrevividoLabel;
    [Export] private Label recordLabel;

    // [----- LÍMITES -----]
    [Export] public double TemperaturaMinimaC = -10;
    [Export] public int MinutosMaximosConFrio = 10;      // minutos marcianos seguidos
    [Export] public int SolesMaximosSinAgua = 3;
    [Export] public int SolesMaximosSinComida = 14;

    private const int MinutosPorSol = 24 * 60;

    // Mejor tiempo sobrevivido. user:// se conserva al reiniciar y al cerrar el juego
    // (en Windows: %APPDATA%\Godot\app_userdata\Mars_JuniorMission_Game\record.cfg).
    private const string RutaRecord = "user://record.cfg";
    private const string SeccionRecord = "record";
    private const string ClaveRecord = "minutos_sobrevividos";

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
        //GD.Print("GAME OVER: " + motivo);

        motivoLabel.Text = motivo;
        MostrarTiempoYRecord();
        Visible = true;
        GetTree().Paused = true;   // este nodo usa process_mode = Always, así que sus botones siguen funcionando
    }



    // [----- TIEMPO SOBREVIVIDO Y RÉCORD -----]
    // El reto es aguantar el mayor número de soles (días marcianos): se muestra el tiempo de esta
    // partida y el mejor de todas. Si se supera, se guarda y se avisa con "NEW RECORD!".
    private void MostrarTiempoYRecord()
    {
        int actual = gameManager.MinutosTranscurridos;
        int record = CargarRecord();

        bool nuevoRecord = actual > record;
        if (nuevoRecord)
            GuardarRecord(actual);

        sobrevividoLabel.Text = "You survived " + FormatearTiempo(actual);
        recordLabel.Text = nuevoRecord ? "NEW RECORD!" : "Best: " + FormatearTiempo(record);
    }

    // Minutos marcianos -> "2 sols, 5 h".
    private static string FormatearTiempo(int minutos)
    {
        int soles = minutos / MinutosPorSol;
        int horas = minutos % MinutosPorSol / 60;

        return $"{soles} {(soles == 1 ? "sol" : "sols")}, {horas} h";
    }

    // Récord guardado en minutos marcianos, o 0 si todavía no hay ninguno.
    private static int CargarRecord()
    {
        var archivo = new ConfigFile();
        if (archivo.Load(RutaRecord) != Error.Ok)
            return 0;

        return (int)archivo.GetValue(SeccionRecord, ClaveRecord, 0);
    }

    private static void GuardarRecord(int minutos)
    {
        var archivo = new ConfigFile();
        archivo.SetValue(SeccionRecord, ClaveRecord, minutos);

        Error error = archivo.Save(RutaRecord);
        if (error != Error.Ok)
            GD.PushWarning("No se pudo guardar el récord: " + error);
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

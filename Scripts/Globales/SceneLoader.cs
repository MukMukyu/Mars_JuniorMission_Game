using Godot;
using System;

/// <summary>
/// Cargador de escenas con pantalla de carga, sin autoload.
/// Uso desde cualquier script:  SceneLoader.LoadScene("uid://...");
///
/// El método estático crea una instancia temporal y la cuelga de la raíz del árbol
/// (no de la escena actual), así sobrevive al cambio de escena. Cuando termina la
/// transición, la instancia se libera sola.
/// </summary>
public partial class SceneLoader : Node
{
    private const string LoadingScreenPath = "uid://djmey06mepf23";
    private const bool UseSubThreads = true;

    // Evita lanzar dos cargas a la vez (doble clic, etc.)
    private static bool IsLoading = false;

    private string ScenePath;
    private LoadingScreen CurrentLoadingScreen;
    private readonly Godot.Collections.Array Progress = new();


    public static void LoadScene(string _ScenePath)
    {
        if (IsLoading)
            return;

        IsLoading = true;

        // Desde un método estático no existe GetTree(), pero el árbol se puede obtener del motor.
        SceneTree Tree = (SceneTree)Engine.GetMainLoop();

        SceneLoader Loader = new SceneLoader();
        Loader.ScenePath = _ScenePath;

        // Diferido para que también funcione si se llama desde un _Ready().
        Tree.Root.CallDeferred(Node.MethodName.AddChild, Loader);
    }


    public override void _Ready()
    {
        // Que siga funcionando aunque el juego esté en pausa.
        ProcessMode = ProcessModeEnum.Always;
        SetProcess(false);

        CurrentLoadingScreen = GD.Load<PackedScene>(LoadingScreenPath).Instantiate<LoadingScreen>();
        CurrentLoadingScreen.LoadingScreenReady += StartLoad;
        CurrentLoadingScreen.LoadingScreenFinished += Finish;
        AddChild(CurrentLoadingScreen);
    }

    public override void _ExitTree()
    {
        IsLoading = false;
    }


    // Se llama cuando la pantalla de carga ya tapó todo (fin del fade in).
    private void StartLoad()
    {
        Error Estado = ResourceLoader.LoadThreadedRequest(ScenePath, "", UseSubThreads);

        if (Estado != Error.Ok)
        {
            Abort($"No se pudo iniciar la carga de '{ScenePath}': {Estado}");
            return;
        }

        SetProcess(true);
    }


    public override void _Process(double delta)
    {
        ResourceLoader.ThreadLoadStatus LoadStatus = ResourceLoader.LoadThreadedGetStatus(ScenePath, Progress);

        if (Progress.Count > 0)
            CurrentLoadingScreen._OnProgressChange(Progress[0].AsSingle());

        switch (LoadStatus)
        {
            case ResourceLoader.ThreadLoadStatus.InvalidResource:
            case ResourceLoader.ThreadLoadStatus.Failed:
                Abort($"Falló la carga de '{ScenePath}': {LoadStatus}");
                break;

            case ResourceLoader.ThreadLoadStatus.Loaded:
                SetProcess(false);
                PackedScene LoadedResource = (PackedScene)ResourceLoader.LoadThreadedGet(ScenePath);
                // Si se cargó desde una pausa (Game Over, menú de pausa), la escena nueva no debe empezar pausada.
                GetTree().Paused = false;
                GetTree().ChangeSceneToPacked(LoadedResource);
                CurrentLoadingScreen._OnLoadFinished();
                break;
        }
    }


    private void Abort(string Mensaje)
    {
        GD.PushError(Mensaje);
        SetProcess(false);
        // Quita la pantalla de carga para no dejar el juego en negro.
        CurrentLoadingScreen._OnLoadFinished();
    }

    // Se llama cuando la pantalla de carga terminó su fade out.
    private void Finish()
    {
        QueueFree(); // también libera la pantalla de carga, que es hija de este nodo
    }
}


using Godot;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

public partial class GameManager : Node
{
    [Export] private AudioStreamPlayer audioSandStorm;
    [Export] private CanvasLayer dustStormLayer;
    [Export] private Timer TimerDia;

    // [----- TIEMPO MARCIANO -----]
    // Un sol (día solar de Marte) dura 24 h 39 min 35 s terrestres (NASA Mars Facts / RMG).
    // El juego lo divide en 24 horas marcianas de 60 minutos: un minuto marciano dura ~61.65 s terrestres.
    public const double SolEnSegundosTerrestres = 88775.244;
    public const double MinutoMarcianoEnSegundos = SolEnSegundosTerrestres / (24 * 60);

    // Los kWh se miden en horas terrestres: cada tick (1 minuto marciano) son ~0.0171 h.
    public const double HorasPorMinutoMarciano = MinutoMarcianoEnSegundos / 3600.0;

    // Cuántas veces más rápido que en Marte corre el reloj del juego.
    // x100: un minuto marciano dura ~0.62 s reales y un sol ~14.8 min reales.
    [Export(PropertyHint.Range, "1,1000")] public double EscalaTiempo = 100;

    // [----- ACTIONS & SIGNALS -----]
    public event Action OnMinutePassed;
    public event Action OnDayPassed;
    public event Action OnDustStorm;

    // [----- OTHERS -----]
    public bool IsDustStorm = false;
    private int DustCounter = 0;   // horas marcianas que lleva la tormenta actual
    private string CurrentAnim;

    // [----- TIEMPO SOBREVIVIDO -----]
    // Minutos marcianos desde que empezó la partida (empieza en el sol 1 a las 00:00).
    // Lo muestra el Game Over junto con el récord: cuanto más aguante el jugador, mejor.
    public int MinutosTranscurridos { get; private set; } = 0;

    // [----- JUGADOR DENTRO / FUERA DE LA BASE -----]
    // Lo cambian las puertas que dan al exterior (PuertaExterior.DaAlExterior). true = dentro.
    public event Action<bool> OnJugadorCambiaZona;
    private bool jugadorEnBase = true;   // la partida empieza dentro de la base
    public bool JugadorEnBase
    {
        get => jugadorEnBase;
        set
        {
            if (value == jugadorEnBase) return;

            jugadorEnBase = value;
            OnJugadorCambiaZona?.Invoke(value);
        }
    }

    private double minutoMarciano = 0;
    public double MinutoMarciano
    {
        get => minutoMarciano;
        set
        {
            if (value >= 60)
            {
                HoraMarciana += 1;
                TormentaDeArena();
                minutoMarciano = 0;
            }
            else
            {
                minutoMarciano = value;
            }
        }
    }
    private double horaMarciana = 0;
    public double HoraMarciana
    {
        get => horaMarciana;
        set
        {
            if (value >= 24)
                horaMarciana = 0;
            else
                horaMarciana = value;
        }
    }
    


    // Segundos marcianos transcurridos del día (0 - 86400). Cada tick del reloj es 1 minuto marciano.
    // Se interpola con el tiempo que lleva el tick en curso para que cambie de forma continua
    // (útil para animar cosas como el cielo) en vez de dar un salto por tick.
    public double SegundoMarciano
    {
        get
        {
            double minutosDelDia = HoraMarciana * 60 + MinutoMarciano;

            double fraccionTick = 0.0;
            if (TimerDia != null && TimerDia.WaitTime > 0 && !TimerDia.IsStopped())
                fraccionTick = 1.0 - TimerDia.TimeLeft / TimerDia.WaitTime;

            return (minutosDelDia + fraccionTick) * 60.0;
        }
    }


    // [----- TORMENTAS DE POLVO -----]
    // Más frecuentes cuanto más polvo hay en la zona (DCI de TES, ConstantesGlobales.DCI):
    //   tormentas por sol = DCI × MultiplicadorTormentas
    // Con el DCI de Gale (≈0.096) y 20 sale ~1.9 tormentas por sol: la primera llega de media
    // a las ~12 h marcianas (~8 min reales a x100).
    [Export] public double MultiplicadorTormentas = 20;
    [Export] public int HorasDeTormenta = 6;   // horas marcianas que dura (6 h = ~3.7 min reales a x100)

    // Sonido: arranca casi en silencio, sube hasta el volumen puesto en el Inspector y al acabar baja y se para.
    private const float VolumenSilencioDb = -40f;
    private float volumenTormentaDb = 0f;

    private Tween tweenAnim { get; set; }

    // Se llama cada hora marciana.
    private void TormentaDeArena()
    {
        if (IsDustStorm)
        {
            DustCounter += 1;
            if (DustCounter >= HorasDeTormenta)
                ApagarTormentaDeArena();
            return;
        }

        double probabilidadPorHora = ConstantesGlobales.DCI * MultiplicadorTormentas / 24.0;
        if (GD.Randf() < probabilidadPorHora)
            EmpezarTormentaDeArena();
    }

    private void EmpezarTormentaDeArena()
    {
        // Empieza transparente y se hace visible con un fundido.
        foreach (Node hijo in dustStormLayer.GetChildren())
            if (hijo is CanvasItem capa)
                capa.Modulate = new Color(capa.Modulate, 0f);

        dustStormLayer.Visible = true;

        if (audioSandStorm != null)
        {
            audioSandStorm.VolumeDb = VolumenSilencioDb;
            audioSandStorm.Play();
        }

        CurrentAnim = "ShowDust";
        AnimarTormenta(1f, Tween.EaseType.Out);

        DustCounter = 0;
        IsDustStorm = true;
        OnDustStorm?.Invoke();
    }
    private void ApagarTormentaDeArena()
    {
        CurrentAnim = "HideDust";
        AnimarTormenta(0f, Tween.EaseType.In);
        IsDustStorm = false;
    }

    // Un CanvasLayer no tiene "modulate": se anima el de cada hijo (los ColorRect del polvo).
    private void AnimarTormenta(float alfaFinal, Tween.EaseType ease)
    {
        ResetTween();
        tweenAnim.SetParallel(true).SetEase(ease).SetTrans(Tween.TransitionType.Sine);

        foreach (Node hijo in dustStormLayer.GetChildren())
            if (hijo is CanvasItem capa)
                tweenAnim.TweenProperty(capa, "modulate:a", alfaFinal, 1.0);

        // El sonido sube o baja a la vez que la imagen.
        if (audioSandStorm != null)
            tweenAnim.TweenProperty(audioSandStorm, "volume_db", alfaFinal > 0f ? volumenTormentaDb : VolumenSilencioDb, 1.0);
    }

    protected void ResetTween()
    {
        if (tweenAnim != null)
            tweenAnim.Kill();

        // Cada tormenta crea un tween nuevo, así que hay que conectarse a cada uno.
        tweenAnim = CreateTween();
        tweenAnim.Finished += _OnAnimFinished;
    }
    private void _OnAnimFinished()
    {
        if (CurrentAnim == "HideDust")
        {
            dustStormLayer.Visible = false;
            audioSandStorm?.Stop();   // tras el fundido ya está en silencio
        }
    }



    public override void _Ready()
    {
        // Cada timeout del TimerDia es un minuto marciano, acelerado por EscalaTiempo.
        TimerDia.Start(MinutoMarcianoEnSegundos / EscalaTiempo);

        // Sonido de la tormenta: el asignado en audioSandStorm o, si no hay ninguno,
        // el AudioStreamPlayer llamado "DustStorm" dentro de la capa DustStorm.
        audioSandStorm ??= dustStormLayer.GetNodeOrNull<AudioStreamPlayer>("DustStorm");
        if (audioSandStorm != null)
        {
            volumenTormentaDb = audioSandStorm.VolumeDb;   // el volumen elegido en el Inspector
            audioSandStorm.Stop();                         // por si tiene Autoplay: solo suena durante la tormenta
        }
        else
        {
            GD.PushWarning("GameManager: no se encontró el sonido de la tormenta (asigna audioSandStorm o llama \"DustStorm\" al AudioStreamPlayer de la capa DustStorm).");
        }
    }

    // Teclas de prueba, solo al jugar desde el editor (build de depuración), para probar o grabar
    // la demo sin esperar al azar:
    //   F9  = empieza o termina una tormenta de polvo
    //   F10 = rompe un panel solar que funcione, elegido al azar
    public override void _UnhandledInput(InputEvent @event)
    {
        if (!OS.IsDebugBuild() || @event is not InputEventKey tecla || !tecla.Pressed || tecla.Echo)
            return;

        if (tecla.Keycode == Key.F9)
        {
            if (IsDustStorm)
                ApagarTormentaDeArena();
            else
                EmpezarTormentaDeArena();
        }
        else if (tecla.Keycode == Key.F10)
        {
            RomperPanelAlAzar();
        }
    }

    private void RomperPanelAlAzar()
    {
        var funcionando = new System.Collections.Generic.List<PanelSolar>();
        foreach (Node nodo in GetTree().GetNodesInGroup(PanelSolar.Grupo))
            if (nodo is PanelSolar panel && panel.Funciona)
                funcionando.Add(panel);

        if (funcionando.Count == 0)
            return;

        funcionando[(int)(GD.Randi() % (uint)funcionando.Count)].Romper();
    }



    private void Reloj()
    {
        MinutoMarciano += 1;
        MinutosTranscurridos += 1;   // antes de avisar, para que el Game Over lea el tiempo ya actualizado

        OnMinutePassed?.Invoke();

        // Medianoche (24:00 -> 00:00): ha pasado un sol completo.
        if (HoraMarciana == 0 && MinutoMarciano == 0)
            OnDayPassed?.Invoke();
    }
}

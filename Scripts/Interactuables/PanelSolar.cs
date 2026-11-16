using Godot;
using System;

public partial class PanelSolar : Interactuable
{
    [Export] private Node AlertBox;
    // Potencia nominal del grupo (kW) con la luz solar de órbita terrestre, como se anuncian
    // los iROSA de la ISS (>20 kW cada uno): 3 paneles x 20 kW = 60 kW.
    // En Marte se multiplica por FactorIrradianciaMarte.
    [Export] public float KiloWattsProducidos = 60f;
    [Export] private Control control;
    [Export] private Label porcentaje;
    [Export] public float PorcentajeDePolvo;

    private bool funciona = true;
    [Export] public bool Funciona
    {
        get => funciona;
        set
        {
            funciona = value;

            // Al repararlo se quita su alerta de "roto".
            if (value)
                CerrarAlerta(ref alertaRoto);

            ActualizarTextoAccion();
        }
    }

    // [----- ALERTAS ACTIVAS (se cierran al resolver el problema) -----]
    private Alerts alertaPolvo;
    private Alerts alertaRoto;

    // [----- REPARACIÓN: MINIJUEGO DE CABLES -----]
    private const string RutaMinijuego = "res://Escenas/GUI/MinijuegoCables.tscn";
    private const string TextoReparar = "Repair the panel";
    private Node minijuegoAbierto;

    // [----- POSICIONES PARA CADA ANIMACIÓN -----]
    protected override Vector2 HIDE_ANIM_POSITION { get; } = new Vector2(-20, 5);
    protected override Vector2 SHOW_ANIM_POSITION { get; } = new Vector2(-20, -20);

    // [----- TEXTO Y ANIMACIÓN DE LA ACCIÓN -----]
    protected override string ACTION_LABEL { get; } = "Clean the panels";
    protected override Vector2 HIDE_ANIM_DIALOG_POSITION { get; } = new Vector2(16, 10);
    protected override Vector2 SHOW_ANIM_DIALOG_POSITION { get; } = new Vector2(16, 0);

    private RandomNumberGenerator rng;
    private float randomSeed;

    public const string Grupo = "paneles";   // para encontrar todos los paneles (p. ej. la tecla F10)
    private static int numPannel = 0;
    private int myNumber = 0;


    // Potencia real en este momento: 0 si está roto, y baja según el polvo acumulado.
    private float potenciaActualkW = 0;
    public float PotenciaActualkW
    {
        get
        {
            if (!Funciona)
                return 0f;

            return potenciaActualkW;
        }
        set
        {
            if (value == potenciaActualkW) return;

            EnergiaManager.ProduccionActualkWh -= potenciaActualkW * (float)GameManager.HorasPorMinutoMarciano;
            EnergiaManager.ProduccionActualkWh += value * (float)GameManager.HorasPorMinutoMarciano;
            potenciaActualkW = value;
        }
    }



    public override void _Ready()
    {
        base._Ready();

        numPannel++;
        myNumber = numPannel;
        AddToGroup(Grupo);

        rng = new RandomNumberGenerator();
        ActualizarTextoAccion();   // por si el panel empieza roto desde el Inspector
    }



    protected override void _DoOnExitTree()
    {
        PotenciaActualkW = 0;
        numPannel--;
    }


    protected override void _DoOnInteract()
    {
        // Roto: E abre el minijuego de cables para repararlo. Funcionando: E lo limpia.
        if (!Funciona)
        {
            AbrirMinijuego();
            return;
        }

        PorcentajeDePolvo = 0;
        porcentaje.Text = "0%";

        // Limpio: se quita su alerta de polvo (y volverá a avisar cuando se llene otra vez).
        CerrarAlerta(ref alertaPolvo);
    }

    // Si el jugador se aleja del panel, el minijuego se cierra sin reparar.
    protected override void _DoOnBodyExited()
    {
        CerrarMinijuego();
    }



    private void AbrirMinijuego()
    {
        if (minijuegoAbierto != null) return;

        // Es un CanvasLayer: aunque cuelgue del panel, se dibuja fijo en pantalla.
        minijuegoAbierto = GD.Load<PackedScene>(RutaMinijuego).Instantiate();
        AddChild(minijuegoAbierto);

        Node cables = minijuegoAbierto.GetNode("Cables");
        cables.Connect("completado", Callable.From(AlRepararCables));
        cables.Connect("cancelado", Callable.From(CerrarMinijuego));
    }

    private void AlRepararCables()
    {
        Funciona = true;   // también quita la alerta "has broken!"

        // Se deja ver el último cable conectado un momento antes de cerrar.
        GetTree().CreateTimer(0.4).Timeout += CerrarMinijuego;
    }

    private void CerrarMinijuego()
    {
        if (minijuegoAbierto != null && IsInstanceValid(minijuegoAbierto))
            minijuegoAbierto.QueueFree();

        minijuegoAbierto = null;
    }

    private void ActualizarTextoAccion()
    {
        if (labelAccion != null && labelAccion.actionText != null)
            labelAccion.actionText.Text = Funciona ? ACTION_LABEL : TextoReparar;
    }

    private static void CerrarAlerta(ref Alerts alerta)
    {
        if (alerta != null && IsInstanceValid(alerta))
            alerta.Cerrar();

        alerta = null;
    }



    protected override void _OnAnimationHide()
    {
        tweenAnim.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Expo);
        tweenAnim.TweenProperty(PopOp, "modulate:a", 0, 0.3);
        tweenAnim.Parallel().TweenProperty(control, "position", HIDE_ANIM_POSITION, 0.3);
    }
    protected override void _OnAnimationShow()
    {
        tweenAnim.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
        tweenAnim.TweenProperty(PopOp, "modulate:a", 1.0f, 0.3);
        tweenAnim.Parallel().TweenProperty(control, "position", SHOW_ANIM_POSITION, 0.3);
    }



    protected override void _OnMinutePassed()
    {
        Empolvarse();
        porcentaje.Text = PorcentajeDePolvo.ToString("F0") + "%";

        Romperse();

        // Potencia = nominal × luz en Marte × altura del Sol × limpieza. Un panel roto no produce.
        float limpieza = 1f - Mathf.Clamp(PorcentajeDePolvo, 0, 100) / 100f;
        float luzMarte = (float)ConstantesGlobales.FactorIrradianciaMarte;
        PotenciaActualkW = Funciona ? KiloWattsProducidos * luzMarte * CalcularFactorSolar() * limpieza : 0f;
    }

    // Fracción de la potencia nominal según la altura del Sol sobre el horizonte en la base
    // (panel plano): 0 de noche y máxima al mediodía. Depende de la latitud y de la estación (Ls):
    //   sin(elevación) = sin(lat)·sin(dec) + cos(lat)·cos(dec)·cos(ángulo horario)
    private float CalcularFactorSolar()
    {
        const double GradARad = Math.PI / 180.0;

        double hora = gameManager.SegundoMarciano / 3600.0;       // hora solar local, 0..24
        double anguloHorario = (hora - 12.0) * 15.0 * GradARad;   // 15° por hora marciana
        double lat = ConstantesGlobales.Latitud * GradARad;
        double dec = ConstantesGlobales.DeclinacionSolar * GradARad;

        double senoElevacion = Math.Sin(lat) * Math.Sin(dec) + Math.Cos(lat) * Math.Cos(dec) * Math.Cos(anguloHorario);
        return (float)Math.Max(0.0, senoElevacion);
    }

    private void Romperse()
    {
        randomSeed = rng.RandfRange(0f, 10000f);

        if (randomSeed < 1 * ConstantesGlobales.DCI)
            Romper();
    }

    // Rompe el panel y avisa con una alerta. La usa el azar (Romperse) y la tecla F10 de GameManager.
    public void Romper()
    {
        if (!Funciona) return;

        string mensaje = "You'll not generate power! Go and fix it!";
        string titulo = "Panel " + myNumber + " has broken!";
        alertaRoto = Alerts.NewAlert(titulo, mensaje);
        AlertBox.AddChild(alertaRoto);

        Funciona = false;
    }

    private void Empolvarse()
    {
        int multiplicadorPolvo = gameManager.IsDustStorm ? 10 : 1;
        PorcentajeDePolvo = Mathf.Min(100f, PorcentajeDePolvo + (float)(multiplicadorPolvo * ConstantesGlobales.DCI));
        GD.Print(PorcentajeDePolvo);

        if (PorcentajeDePolvo >= 100 && alertaPolvo == null)
        {
            string mensaje = "You'll generate less power! Go and clean them!";
            string titulo = "Panel " + myNumber + " is dusty!";
            alertaPolvo = Alerts.NewAlert(titulo, mensaje);
            AlertBox.AddChild(alertaPolvo);
        }
    }
}

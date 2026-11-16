using System;
using Godot;

// Temperatura dentro de la base.
//  - Con energía: la calefacción mantiene TemperaturaObjetivo y gasta más cuanto más frío hace fuera
//    (pérdida de calor proporcional a la diferencia con el exterior).
//  - Sin energía: la base se va igualando poco a poco con el exterior (ley de enfriamiento de Newton).
// La temperatura exterior es la del aire que da el Mars Climate Database (TemperaturaMCD.AireC).
public partial class TemperaturaInterna : Node
{
    [Export] private Label etiqueta;

    // [----- CALEFACCIÓN -----]
    [Export] public double TemperaturaObjetivo = 21.0;     // °C que se quieren dentro de la base
    [Export] public double KiloWattsPorGrado = 0.02;       // kW que cuesta cada °C de diferencia con el exterior
    [Export] public float ReencenderConkWh = 2f;           // carga mínima para volver a encender tras quedarse sin batería

    // [----- INERCIA TÉRMICA (horas marcianas para recorrer ~63 % de la diferencia) -----]
    [Export] public double HorasEnfriamiento = 4.0;        // sin energía, hacia la temperatura exterior
    [Export] public double HorasCalentamiento = 0.5;       // con energía, de vuelta a TemperaturaObjetivo

    public static double InteriorC { get; private set; }
    public static bool CalefaccionEncendida { get; private set; } = true;

    private const double HorasPorTick = 1.0 / 60.0;        // cada tick del reloj es un minuto marciano

    private GameManager _gameManager;

    private float consumoActualkW = 0;
    private float ConsumoActualkW
    {
        get => consumoActualkW;
        set
        {
            if (value == consumoActualkW) return;

            EnergiaManager.ConsumoActualkWh -= consumoActualkW * (float)GameManager.HorasPorMinutoMarciano;
            EnergiaManager.ConsumoActualkWh += value * (float)GameManager.HorasPorMinutoMarciano;
            consumoActualkW = value;
        }
    }



    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(GetTree().CurrentScene.GetPath());
        _gameManager.OnMinutePassed += Actualizar;

        // La partida empieza con la base climatizada.
        InteriorC = TemperaturaObjetivo;
        CalefaccionEncendida = true;
        Mostrar();
    }

    public override void _ExitTree()
    {
        if (_gameManager != null) _gameManager.OnMinutePassed -= Actualizar;

        // EnergiaManager es estático: hay que quitar lo que gastaba la calefacción.
        ConsumoActualkW = 0;
    }



    private void Actualizar()
    {
        double exteriorC = TemperaturaMCD.AireC;

        // Se apaga al vaciarse la batería y no vuelve hasta tener un mínimo de carga,
        // para que no se encienda y apague en cada tick con la batería en 0.
        if (EnergiaManager.CargaActualkWh <= 0)
            CalefaccionEncendida = false;
        else if (EnergiaManager.CargaActualkWh >= ReencenderConkWh)
            CalefaccionEncendida = true;

        if (CalefaccionEncendida)
        {
            InteriorC = Acercar(InteriorC, TemperaturaObjetivo, HorasCalentamiento);
            ConsumoActualkW = (float)(KiloWattsPorGrado * Math.Abs(TemperaturaObjetivo - exteriorC));
        }
        else
        {
            InteriorC = Acercar(InteriorC, exteriorC, HorasEnfriamiento);
            ConsumoActualkW = 0;
        }

        Mostrar();
    }

    // Cada tick recorre una fracción fija de la distancia al objetivo: rápido al principio y cada vez más lento.
    private static double Acercar(double actual, double objetivo, double constanteHoras)
    {
        double fraccion = 1.0 - Math.Exp(-HorasPorTick / constanteHoras);
        return actual + (objetivo - actual) * fraccion;
    }

    private void Mostrar()
    {
        if (etiqueta != null) etiqueta.Text = $"{InteriorC:0.0} °C";
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Godot;


public partial class TemperaturaMCD : Node
{
    private const string UrlBase = "https://www-mars.lmd.jussieu.fr/mcd_python/";

    [Export] public double Latitud = -4.6;     // Grados norte (negativo = sur). Por defecto: cráter Gale.
    [Export] public double Longitud = 137.4;   // Grados este.
    [Export(PropertyHint.Range, "0,360")] public double Ls = 90.0; // Longitud solar = estación del año marciano.
    [Export] private Label etiqueta;           // Opcional: muestra la temperatura del aire.

    public static double AireC { get; private set; }
    public static double SueloC { get; private set; }

    // 25 valores en Kelvin: índice = hora local marciana (0..24; el 24 repite al 0).
    // Respaldo: MCD v6.2, Lat -4.6, Lon 137.4, Ls 90, escenario climatológico.
    private double[] _sueloK =
    {
        188.0, 186.7, 185.8, 184.8, 184.0, 183.1, 186.1, 189.8, 205.2, 222.8, 237.1, 250.7, 256.3,
        260.3, 256.7, 251.7, 239.2, 225.3, 213.7, 202.5, 197.5, 193.7, 191.4, 189.4, 188.0
    };
    private double[] _aireK =
    {
        194.0, 192.2, 191.0, 189.9, 189.0, 188.1, 189.1, 190.5, 199.1, 209.1, 217.1, 224.7, 229.0,
        232.8, 234.2, 235.0, 231.7, 227.6, 218.9, 209.4, 204.6, 200.8, 198.2, 195.9, 194.0
    };

    private GameManager _gameManager;
    private HttpRequest _http;
    private bool _esperandoArchivo;   // false = esperando el HTML de la consulta; true = esperando el .txt

    private string RutaCache => string.Format(CultureInfo.InvariantCulture,
        "user://mcd_{0:0.##}_{1:0.##}_{2:0.##}.txt", Latitud, Longitud, Ls);

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(GetTree().CurrentScene.GetPath());
        _gameManager.OnSecondPassed += Actualizar;

        if (FileAccess.FileExists(RutaCache))
        {
            using var archivo = FileAccess.Open(RutaCache, FileAccess.ModeFlags.Read);
            if (Cargar(archivo.GetAsText()))
            {
                GD.Print("MCD: datos cargados desde " + RutaCache);
                Actualizar();
                return;
            }
        }

        Actualizar(); // Mientras llega la respuesta se usa el ciclo de respaldo.
        Descargar();
    }

    public override void _ExitTree()
    {
        if (_gameManager != null) _gameManager.OnSecondPassed -= Actualizar;
    }

    private void Actualizar()
    {
        double hora = _gameManager.HoraMarciana;
        AireC = Interpolar(_aireK, hora) - 273.15;
        SueloC = Interpolar(_sueloK, hora) - 273.15;

        if (etiqueta != null) etiqueta.Text = $"{AireC:0.0} °C";
    }

    private static double Interpolar(double[] porHora, double hora)
    {
        hora = Math.Clamp(hora, 0.0, 24.0);
        int h0 = Math.Min((int)Math.Floor(hora), 23);
        double t = hora - h0;
        return porHora[h0] + (porHora[h0 + 1] - porHora[h0]) * t;
    }

    // [---------- DESCARGA ----------]

    private void Descargar()
    {
        _http = new HttpRequest { Timeout = 90 }; // MCD tarda varios segundos en calcular
        AddChild(_http);
        _http.RequestCompleted += AlTerminar;

        // Hay que mandar TODOS los campos del formulario (aunque vayan vacíos);
        // si falta alguno el servidor devuelve un archivo viejo de otra consulta.
        string url = string.Format(CultureInfo.InvariantCulture,
            UrlBase + "cgi-bin/mcdcgi.py?var1=tsurf&var2=t&var3=none&var4=none&datekeyhtml=1" +
            "&ls={0}&localtime=all&year=&month=&day=&hours=&minutes=&seconds=&julian=&martianyear=&sol=" +
            "&latitude={1}&longitude={2}&altitude=2&zkey=3&spacecraft=none&betatest=on&isfixedlt=off" +
            "&dust=1&hrkey=1&averaging=off&zonmean=off&diumean=off&animation=off&animframes=12&dpi=80" +
            "&islog=off&colorm=jet&minval=&maxval=&proj=cyl&palt=&plon=&plat=&trans=&iswind=off" +
            "&latpoint=&lonpoint=",
            Ls, Latitud, Longitud);

        _esperandoArchivo = false;
        Error error = _http.Request(url);
        if (error != Error.Ok) GD.PushWarning("MCD: no se pudo enviar la petición (" + error + "); se usa el respaldo.");
    }

    private void AlTerminar(long resultado, long codigo, string[] encabezados, byte[] cuerpo)
    {
        if (resultado != (long)HttpRequest.Result.Success || codigo != 200)
        {
            GD.PushWarning($"MCD: falló la descarga (resultado {resultado}, código {codigo}); se usa el respaldo.");
            return;
        }

        string texto = Encoding.UTF8.GetString(cuerpo);

        if (!_esperandoArchivo)
        {
            // Paso 1: el HTML trae un enlace tipo ../txt/<hash>.txt con los datos.
            Match enlace = Regex.Match(texto, @"txt/[a-f0-9]+\.txt");
            if (!enlace.Success)
            {
                GD.PushWarning("MCD: la respuesta no trae archivo de datos; se usa el respaldo.");
                return;
            }

            _esperandoArchivo = true;
            Error error = _http.Request(UrlBase + enlace.Value);
            if (error != Error.Ok) GD.PushWarning("MCD: no se pudo pedir el archivo (" + error + ").");
            return;
        }

        // Paso 2: el archivo de texto con las dos tablas (suelo y aire).
        if (!Cargar(texto))
        {
            GD.PushWarning("MCD: no se pudo leer el archivo de datos; se usa el respaldo.");
            return;
        }

        using (var archivo = FileAccess.Open(RutaCache, FileAccess.ModeFlags.Write))
            archivo?.StoreString(texto);

        GD.Print($"MCD: temperaturas descargadas (Lat {Latitud}, Lon {Longitud}, Ls {Ls}) y guardadas en {RutaCache}");
        Actualizar();
    }

    // [--------- CARGAR ---------]
    private bool Cargar(string texto)
    {
        string esperado = string.Format(CultureInfo.InvariantCulture, "Latitude {0:0.0#}N. Longitude {1:0.0#}E.", Latitud, Longitud);
        if (!texto.Contains(esperado) || !texto.Contains("Local time (Martian hour)"))
            return false;

        var bloques = new List<List<double>>();
        bool enDatos = false;

        foreach (string linea in texto.Split('\n'))
        {
            string l = linea.Trim();
            if (l.Length == 0) continue;
            if (l.StartsWith('#')) { enDatos = false; continue; }

            string[] columnas = l.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            if (columnas.Length < 2 ||
                !double.TryParse(columnas[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double valor))
                return false;

            if (!enDatos) { bloques.Add(new List<double>()); enDatos = true; }
            bloques[^1].Add(valor);
        }

        if (bloques.Count < 2 || bloques[0].Count != 25 || bloques[1].Count != 25)
            return false;

        _sueloK = bloques[0].ToArray();
        _aireK = bloques[1].ToArray();
        return true;
    }
}

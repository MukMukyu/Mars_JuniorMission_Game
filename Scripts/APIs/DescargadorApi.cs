using System;
using System.Text;
using System.Text.Json;
using Godot;

/// <summary>
/// OPCIONAL: ejemplo de cómo pedir datos a una API de la NASA desde Godot.
/// El juego NO lo necesita (ya trae los datos en datos/), pero sirve para aprender
/// o para descargar datos nuevos antes de la presentación.
///
/// Uso: agreguen un Node a cualquier escena, pónganle este script, escriban su
/// API key en el Inspector (se saca gratis en https://api.nasa.gov) y den Play.
/// Los resultados salen en la pestaña "Salida" de Godot.
/// </summary>
public partial class DescargadorApi : Node
{
    [Export] public string ApiKey = "DEMO_KEY";   // DEMO_KEY solo permite ~30 consultas por hora
    [Export] public string FechaInicio = "2024-05-01";
    [Export] public string FechaFin = "2024-05-31"; // DONKI acepta máximo 60 días por consulta

    private HttpRequest _http;

    public override void _Ready()
    {
        _http = new HttpRequest();
        AddChild(_http);
        _http.RequestCompleted += AlTerminar;

        // GST = tormentas geomagnéticas (afectan a la Tierra). Otras opciones: CME, SEP.
        string url = $"https://api.nasa.gov/DONKI/GST?startDate={FechaInicio}&endDate={FechaFin}&api_key={ApiKey}";
        GD.Print("Pidiendo: " + url.Replace(ApiKey, "***"));

        Error error = _http.Request(url);
        if (error != Error.Ok) GD.PushError("No se pudo enviar la petición: " + error);
    }

    private void AlTerminar(long resultado, long codigo, string[] encabezados, byte[] cuerpo)
    {
        string texto = Encoding.UTF8.GetString(cuerpo);
        if (codigo != 200)
        {
            // 429 = se acabaron las consultas de la hora; 400 = fechas mal escritas o rango > 60 días
            GD.PushError($"La API respondió {codigo}: {texto}");
            return;
        }

        // Guardamos la respuesta para usarla después sin internet (user:// es una carpeta del jugador)
        using (var archivo = FileAccess.Open("user://donki_gst.json", FileAccess.ModeFlags.Write))
            archivo.StoreString(texto);

        // Leer el JSON: es una lista [ {...}, {...} ]
        using JsonDocument doc = JsonDocument.Parse(texto);
        foreach (JsonElement tormenta in doc.RootElement.EnumerateArray())
        {
            string inicio = tormenta.GetProperty("startTime").GetString();
            double kpMax = 0;
            foreach (JsonElement kp in tormenta.GetProperty("allKpIndex").EnumerateArray())
                kpMax = Math.Max(kpMax, kp.GetProperty("kpIndex").GetDouble());

            // Kp va de 0 a 9; 5 o más ya es tormenta.
            GD.Print($"Tormenta geomagnética {inicio}: Kp máximo {kpMax}");
        }
        GD.Print("Guardado en user://donki_gst.json");
    }
}

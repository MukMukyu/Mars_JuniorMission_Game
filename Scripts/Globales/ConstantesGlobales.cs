using System;
using Godot;

public class ConstantesGlobales
{
    // [----- ESCENAS (para SceneLoader.LoadScene) -----]
    public const string EscenaMenu = "uid://dlhdkwymxnwsf";      // Escenas/GUI/menu.tscn
    public const string EscenaPartida = "uid://dkedj5plii0xj";   // Escenas/Principales/MundoPrincipal.tscn


    // [----- GRAVEDAD DE MARTE -----]
    public static float Gravedad = 37.1f;


    // [----- LATITUD y LONGITUD (Marte) -----]
    public static double Latitud = -4.6;
    public static double Longitud = 137.4;


    // [----- ESTACIÓN DEL AÑO (Longitud solar, Ls) -----]
    // Ls 90 = solsticio de invierno del hemisferio sur. Es el mismo escenario que se le pide al MCD
    // (TemperaturaMCD.Ls): si se cambia aquí, cambiarlo también allí.
    public static double LongitudSolar = 90;

    // [----- LUZ SOLAR EN MARTE -----]
    // Irradiancia solar media: 586.2 W/m² en Marte frente a 1361 W/m² en la Tierra (NASA Mars Fact Sheet).
    // Un panel rinde en proporción: un iROSA de 20 kW en órbita terrestre daría ~8.6 kW en Marte.
    public const double FactorIrradianciaMarte = 586.2 / 1361.0;

    // Inclinación del eje de Marte: 25.19° (NASA Mars Fact Sheet).
    public const double OblicuidadMarte = 25.19;

    // Declinación del Sol en grados para esa estación: sin(dec) = sin(oblicuidad) · sin(Ls).
    public static double DeclinacionSolar =>
        Math.Asin(Math.Sin(OblicuidadMarte * Math.PI / 180.0) * Math.Sin(LongitudSolar * Math.PI / 180.0)) * 180.0 / Math.PI;


    // [----- INDICE DE RECUBRIMIENTO DE POLVO (DCI) -----]
    private const double HueInicialValue = 270;
    private const double DCIHueValue = 244;
    public static double DCI = 1 - (DCIHueValue / HueInicialValue);
}

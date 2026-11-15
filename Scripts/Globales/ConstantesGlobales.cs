using System;
using Godot;

public class ConstantesGlobales
{
    // [----- GRAVEDAD DE MARTE -----]
    public static float Gravedad = 37.1f;


    // [----- LATITUD y LONGITUD (Marte) -----]
    public static double Latitud = -4.6;
    public static double Longitud = 137.4;


    // [----- INDICE DE RECUBRIMIENTO DE POLVO (DCI) -----]
    private const double HueInicialValue = 270;
    private const double DCIHueValue = 244;
    public static double DCI = 1 - (DCIHueValue / HueInicialValue);
}

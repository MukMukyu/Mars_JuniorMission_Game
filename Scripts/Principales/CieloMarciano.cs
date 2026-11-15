using Godot;

// Alimenta el shader CieloMarciano.gdshader con la hora marciana del GameManager.
public partial class CieloMarciano : ColorRect
{
    // Para previsualizar el cielo sin esperar al reloj: si es >= 0 se usa esta hora (0-24)
    // en lugar de GameManager.SegundoMarciano. Déjalo en -1 para el comportamiento normal.
    [Export(PropertyHint.Range, "-1,24,0.1")] public float HoraForzada { get; set; } = -1f;

    private GameManager gameManager;
    private ShaderMaterial material;

    public override void _Ready()
    {
        gameManager = GetTree().CurrentScene as GameManager;
        material = Material as ShaderMaterial;

        MouseFilter = MouseFilterEnum.Ignore;

        if (material == null)
            GD.PushWarning($"{Name}: necesita un ShaderMaterial con CieloMarciano.gdshader.");
    }

    public override void _Process(double delta)
    {
        if (material == null)
            return;

        float hora;
        if (HoraForzada >= 0f)
            hora = HoraForzada;
        else if (gameManager != null)
            hora = (float)(gameManager.SegundoMarciano / 3600.0);
        else
            return;

        material.SetShaderParameter("hora", hora);
    }
}

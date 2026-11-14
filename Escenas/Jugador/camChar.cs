using Godot;

public partial class camChar : Camera2D
{
    [Export] public float zoomNormal = 1.0f;
    [Export] public float zoomSalto = 0.75f;
    [Export] public float velocidadZoom = 3.0f;

    public override void _Ready()
    {
        Zoom = new Vector2(zoomNormal, zoomNormal);
    }

    public override void _Process(double delta)
    {
        
        CharacterBody2D player = GetParent<CharacterBody2D>();

        bool saltando = !player.IsOnFloor();

        float zoomObjetivo = saltando ? zoomSalto : zoomNormal;

        float nuevoZoom = Mathf.Lerp(
            Zoom.X,
            zoomObjetivo,
            velocidadZoom * (float)delta
        );

        Zoom = new Vector2(nuevoZoom, nuevoZoom);
    }
}

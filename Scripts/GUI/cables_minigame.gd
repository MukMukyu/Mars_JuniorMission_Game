extends Control

# Minijuego de cables para reparar un panel solar roto.
# Hay que unir cada cable de la izquierda con el de su color a la derecha:
# se puede arrastrar de uno a otro, o hacer clic en uno y luego en su pareja.
# Al conectar los 4 emite "completado"; con ESC emite "cancelado".
# Quien lo abre (PanelSolar.cs) escucha esas señales y lo cierra.

signal completado
signal cancelado

const GROSOR := 4.0
const TOTAL_CABLES := 4

var conexiones = []
var colores_conectados = []
var cable_activo = false
var inicio_pos = Vector2.ZERO
var color_actual = Color.WHITE

@onready var extremos_derechos = {
	$VBoxContainer2/GreenRight: Color.GREEN,
	$VBoxContainer2/BlueRight: Color.BLUE,
	$VBoxContainer2/YellowRight: Color.YELLOW,
	$VBoxContainer2/RedRight: Color.RED,
}


func _ready():
	# Cada reparación mezcla el orden de los cables.
	for columna in [$VBoxContainer, $VBoxContainer2]:
		var hijos = columna.get_children()
		hijos.shuffle()
		for i in hijos.size():
			columna.move_child(hijos[i], i)


func _draw():
	for conexion in conexiones:
		draw_line(conexion[0], conexion[1], conexion[2], GROSOR)

	if cable_activo:
		draw_line(inicio_pos, get_local_mouse_position(), color_actual, GROSOR)

func _process(_delta):
	if cable_activo:
		queue_redraw()

func _unhandled_input(event):
	if event.is_action_pressed("ui_cancel"):
		get_viewport().set_input_as_handled()
		cancelado.emit()


func _on_green_left_gui_input(event: InputEvent) -> void:
	manejar_clic_inicio(event, $VBoxContainer/GreenLeft, Color.GREEN)

func _on_blue_left_gui_input(event: InputEvent) -> void:
	manejar_clic_inicio(event, $VBoxContainer/BlueLeft, Color.BLUE)

func _on_yellow_left_gui_input(event: InputEvent) -> void:
	manejar_clic_inicio(event, $VBoxContainer/YellowLeft, Color.YELLOW)

func _on_red_left_gui_input(event: InputEvent) -> void:
	manejar_clic_inicio(event, $VBoxContainer/RedLeft, Color.RED)


func _on_green_right_gui_input(event: InputEvent) -> void:
	manejar_clic_fin(event, $VBoxContainer2/GreenRight, Color.GREEN)

func _on_blue_right_gui_input(event: InputEvent) -> void:
	manejar_clic_fin(event, $VBoxContainer2/BlueRight, Color.BLUE)

func _on_yellow_right_gui_input(event: InputEvent) -> void:
	manejar_clic_fin(event, $VBoxContainer2/YellowRight, Color.YELLOW)

func _on_red_right_gui_input(event: InputEvent) -> void:
	manejar_clic_fin(event, $VBoxContainer2/RedRight, Color.RED)


func manejar_clic_inicio(event: InputEvent, nodo: Control, color: Color):
	if not (event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT):
		return

	if event.pressed:
		if color in colores_conectados:
			return
		cable_activo = true
		# El cable sale del borde interior (derecho) del extremo izquierdo.
		inicio_pos = Vector2(nodo.global_position.x + nodo.size.x, nodo.global_position.y + nodo.size.y / 2) - global_position
		color_actual = color
	elif cable_activo:
		# Al soltar el botón: si arrastró hasta un extremo derecho, se intenta conectar ahí.
		# Si lo soltó sobre el mismo cable (fue un clic), sigue activo para el modo clic-clic.
		var destino = extremo_bajo_raton()
		if destino != null:
			conectar(destino, extremos_derechos[destino])

func manejar_clic_fin(event: InputEvent, nodo: Control, color_esperado: Color):
	if event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT and cable_activo:
		conectar(nodo, color_esperado)


func extremo_bajo_raton():
	for nodo in extremos_derechos:
		if nodo.get_global_rect().has_point(nodo.get_global_mouse_position()):
			return nodo
	return null

func conectar(nodo: Control, color_destino: Color):
	# Si el color no coincide, el cable se suelta y hay que volver a intentarlo.
	if color_actual == color_destino and color_actual not in colores_conectados:
		# Llega al borde interior (izquierdo) del extremo derecho.
		var fin_pos = Vector2(nodo.global_position.x, nodo.global_position.y + nodo.size.y / 2) - global_position
		conexiones.append([inicio_pos, fin_pos, color_actual])
		colores_conectados.append(color_actual)

	cable_activo = false
	queue_redraw()

	if colores_conectados.size() == TOTAL_CABLES:
		completado.emit()

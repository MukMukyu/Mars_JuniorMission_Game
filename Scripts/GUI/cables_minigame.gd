extends Control

var conexiones = []
var cable_activo = false
var inicio_pos = Vector2.ZERO
var color_actual = Color.WHITE
 

func _draw():
	for conexion in conexiones:
		draw_line(conexion[0], conexion[1], conexion[2], 8.0)
	
	if cable_activo:
		draw_line(inicio_pos, get_local_mouse_position(), color_actual, 8.0)

func _process(_delta):
	if cable_activo:
		queue_redraw()


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
	if event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		cable_activo = true
		var centro_nodo = nodo.global_position + (nodo.size / 2)
		inicio_pos = centro_nodo - global_position
		color_actual = color

func manejar_clic_fin(event: InputEvent, nodo: Control, color_esperado: Color):
	if event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		if cable_activo and color_actual == color_esperado:
			var centro_nodo = nodo.global_position + (nodo.size / 2)
			var fin_pos = centro_nodo - global_position
			conexiones.append([inicio_pos, fin_pos, color_actual])
		
		cable_activo = false
		queue_redraw()

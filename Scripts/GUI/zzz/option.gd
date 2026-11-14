extends Control

var sonido_activado = true


func _ready():
	print("OPTION CARGADA")


func _on_quit_pressed() -> void:
	get_tree().quit()

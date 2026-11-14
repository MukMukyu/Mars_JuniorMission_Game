extends Control


func _ready():
	print("MENU CARGADO")

func _on_play_pressed() -> void:
	get_tree().change_scene_to_file

func _on_option_pressed() -> void:
	get_tree().change_scene_to_file("res://escenes/option.tscn")

func _on_quit_pressed() -> void:
	get_tree().quit()

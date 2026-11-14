extends Control

func pause():
	get_tree().paused=!get_tree().paused
	visible=get_tree().paused

func _input(event):
	if event.is_action_pressed("Pause"):
		pause()


func _on_continue_pressed() -> void:
	pause().get_tre


func _on_restart_pressed() -> void:
	get_tree().paused=false
	get_tree().reload_current_scene()


func _on_menu_pressed() -> void:
	get_tree().paused=false
	get_tree().change_scene_to_file("res://escenes/menu.tscn")


func _on_quit_pressed() -> void:
	get_tree().quit()

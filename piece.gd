extends StaticBody2D

signal dropped
signal lifted

var can_drag = false
var dragging = false


func _input(event):
			
	if can_drag && event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		dragging = true
		lifted.emit(self)
		
	elif dragging && event is InputEventMouseButton and event.is_released and event.button_index == MOUSE_BUTTON_LEFT:
		dragging = false
		dropped.emit(self)
			
	elif event is InputEventMouseMotion && dragging:
		position = event.position

func _on_mouse_shape_entered(_shape_idx):
	if dragging: 
		return
	can_drag = true

func _on_mouse_shape_exited(_shape_idx):
	if dragging: 
		return
	can_drag = false

extends Area2D
class_name ShadowDrop

var Direction: Vector2
var Speed: float

# Called when the node enters the scene tree for the first time.
func _ready():
	Direction = Vector2(randf_range(-1,1),randf_range(-1,1)).normalized()
	Speed = randf_range(0,50)
	


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	position += Direction*Speed*delta
	Speed *= 0.98
	if Speed <= 1:
		Speed = 0

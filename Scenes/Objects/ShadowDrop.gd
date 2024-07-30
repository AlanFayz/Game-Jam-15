extends Area2D

var Direction: Vector2
var Speed: float

# Called when the node enters the scene tree for the first time.
func _ready():
	Direction = Vector2(randf_range(-1,1),randf_range(-1,1)).normalized()
	print(Direction)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

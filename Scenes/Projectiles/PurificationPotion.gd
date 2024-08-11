extends Area2D

signal CreatePurificationPool(pos, radius)

var Radius: float
var Speed: float
var Direction: Vector2

# Called when the node enters the scene tree for the first time.
func _ready():
	pass


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	position += Speed*Direction*delta


func OnBreakTimerTimeout():
	CreatePurificationPool.emit(global_position, Radius)
	queue_free()

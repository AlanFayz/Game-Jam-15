extends Area2D

signal PurifyArea(pos, radius)
var Radius: float

# Called when the node enters the scene tree for the first time.
func _ready():
	print("Created pool")
	$GPUParticles2D.emitting = true
	PurifyArea.emit(global_position, Radius)
	
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	pass


func OnGPUParticles2DFinished():
	queue_free()

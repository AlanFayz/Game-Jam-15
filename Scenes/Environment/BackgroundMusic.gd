extends AudioStreamPlayer2D


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	var volume = float(Globals.MusicVolume) 
	self.volume_db = volume - 70
	pass


func OnFinished():
	play()
	pass # Replace with function body.

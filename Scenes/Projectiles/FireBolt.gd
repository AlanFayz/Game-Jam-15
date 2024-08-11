extends Area2D

var Damage = null
var Speed = null
var Direction = null

# Called when the node enters the scene tree for the first time.
func _ready():
	self.look_at(Direction+self.global_position)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	self.position += Direction*Speed*delta;

func OnTimerTimeout():
	self.queue_free()

func OnBodyEntered(body):
	if "Hit" in body:
		body.Hit(self, Damage, [0,0,1])
	
	self.queue_free()


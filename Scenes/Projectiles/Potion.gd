extends Area2D

signal PotionBreak(pos, poolDamage, potionType);

var Direction = null
var Speed = null
var PoolDamage = null
var BreakDamage = null
var PotionType = null

func _ready():
	pass # Replace with function body.


func _process(delta):
	self.position += Direction*Speed*delta
	pass
	
func OnBreakTimerTimeout():
	Break();


func OnBodyEntered(body):
	if body is IHittable:
		body.Hit(self, BreakDamage, [0, 0, 0])
	
	Break();


func Break():
	emit_signal(PotionBreak.get_name(), self.global_position, PoolDamage, PotionType);
	queue_free();

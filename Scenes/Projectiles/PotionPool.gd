extends Area2D


var Damage = 0
var PotionType = null
var Effects = [0, 0, 0]

func _ready():
	if PotionType[0]>0:
		set_collision_layer_value(3, true)
		set_collision_layer_value(6, false)
		set_collision_layer_value(1,  false)
		Damage *= 0.7
	
	$Timer.start(5+PotionType[1])

	for i in range(2, 5):
		Effects[i-2] = PotionType[i]



func OnTimerTimeout():
		queue_free()
	
func OnBodyEntered(body):
		if body is IHittable:
			print(body)
			body.Hit(self, Damage, Effects)
		
	

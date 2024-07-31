extends Area2D

signal PotionBreak(pos, poolDamage, potionType)

var Direction = null
var Speed = null
var PoolDamage = null
var BreakDamage = null
var PotionType = null

@onready var PotionSprite = $SpinPivot/PotionSprite

func _ready():
	SetVisualEffects()
	
func SetVisualEffects():
	var ChosenType = PotionType.max()
	print(ChosenType)

	match ChosenType:
		0:
			PotionSprite.frame = 67
		1:
			pass
			#As Endurance doesent have a particle emmitter, it is not needed
		2:
			PotionSprite.frame = 73
		3:
			PotionSprite.frame = 71
		4:
			PotionSprite.frame = 69
		_:
			PotionSprite.frame = 66


func _process(delta):
	self.position += Direction*Speed*delta
	pass
	
func OnBreakTimerTimeout():
	Break();


func OnBodyEntered(body):
	if "Hit" in body:
		body.Hit(self, BreakDamage, [0, 0, 0])
	
	Break();


func Break():
	emit_signal(PotionBreak.get_name(), self.global_position, PoolDamage, PotionType);
	queue_free();

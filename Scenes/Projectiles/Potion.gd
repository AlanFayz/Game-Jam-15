extends Area2D

signal PotionBreak(pos, poolDamage, potionType)

var Direction = null
var Speed = null
var PoolDamage = null
var BreakDamage = null
var PotionType = null

func _ready():
	SetVisualEffects()
	
func SetVisualEffects():
	var particleTypeNum: int = 0
	var activeTypes = []
	var ChosenType = PotionType.max()
	print(ChosenType)
	var ChosenColour = Color()

	match ChosenType:
		0:
			ChosenColour = Color("#0195ae")
		1:
			pass
			#As Endurance doesent have a particle emmitter, it is not needed
		2:
			ChosenColour = Color("#60FBFF")
		3:
			ChosenColour = Color("#FF4963")
		4:
			ChosenColour = Color("#CAFFC5")
	print(ChosenColour)
	if ChosenColour != Color():
		$SpinPivot/PotionSprite.modulate = ChosenColour
	else:
		$SpinPivot/PotionSprite.frame = 66


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

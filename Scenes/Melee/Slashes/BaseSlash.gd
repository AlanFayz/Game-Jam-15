extends Area2D

class_name BaseSlash

var AttackDir: Vector2
var Damage: float
var SlashSpeed: float

func SetSpeed() -> float:
	return 3

func _ready():
	SlashSpeed = SetSpeed()

	look_at(AttackDir+self.global_position)
	
	if AttackDir.x<0:
		scale.y *= -1
	
	if (randi()%2==0):
		$AnimationPlayer.play("Slash", -1, SlashSpeed)
	else:
		$AnimationPlayer.play("UpSlash", -1, SlashSpeed)


func OnBodyEntered(body):
	if body is IHittable:
		body.Hit(self, Damage, [0,0,0,0,0])
	

func SlashEnd():
	queue_free()


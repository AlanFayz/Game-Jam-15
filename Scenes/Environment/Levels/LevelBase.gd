extends Node2D


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


func OnPlayerPotionThrow(position: Vector2, direction: Vector2, speed: float, breakDamage: float, poolDamage: float):
	var potion = preload("res://Scenes/Projectiles/Potion.tscn").instantiate()
	potion.Position = position
	potion.Speed = speed
	potion.Direction = direction
	potion.PoolDamage = poolDamage
	potion.BreakDamage = breakDamage
	$Projectiles.add_child(potion)
	potion.connect("PotionBreak", OnPotionPotionBreak)

func OnPotionPotionBreak(position: Vector2, poolDamage: float):
	var potionPool = preload("res://Scenes/Projectiles/PotionPool.tscn").instantiate()
	potionPool.position = position
	potionPool.Damage = poolDamage
	$Projectiles.call_deferred("add_child", potionPool)

func OnPlayerSlash(slashPosition: Vector2, slashDirection: Vector2, slashDamage: float, slashType: String):
	var slash = load(slashType)
	slash.Position = slashPosition
	slash.AttackDir = slashDirection
	slash.Damage = slashDamage
	add_child(slash)

func OnEnemyFireBolt(position: Vector2, direction: Vector2, speed: float, damage: float):
	var fireBolt = preload("res://Scenes/Projectiles/FireBolt.tscn").instantiate()
	fireBolt.global_position = position
	fireBolt.direction = direction
	fireBolt.Speed = speed
	fireBolt.Damage = damage
	$Projectiles.call_deferred("add_child",fireBolt)

func OnPlayerPlayerDeath():
	print("Player Has Died")


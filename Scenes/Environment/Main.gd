extends Node

class GameComponents:
	var RandomNumbers: RandomNumberGenerator
	
var m_GameComponents = null
var m_SpawnZone = null

func _ready():
	m_GameComponents = GameComponents.new()
	m_GameComponents.RandomNumbers = RandomNumberGenerator.new()


func OnPlayerPotionThrow(position, direction, speed, breakDamage, poolDamage, potionType):
	var potion = ResourceLoader.load("res://Scenes/Projectiles/Potion.tscn")
	potion.Position = position
	potion.Speed = speed
	potion.Direction = direction
	potion.PoolDamage = poolDamage
	potion.BreakDamage = breakDamage
	potion.PotionType = potionType
	$Projectiles.add_child(potion)
	potion.connect("PotionBreak", OnPotionPotionbreak)
	pass

func OnPotionPotionbreak(position, poolDamage, potionType):
	var potionPool = ResourceLoader.load("res://Scenes/Projectiles/PotionPool.tscn")
	potionPool.Position = position
	potionPool.Damage = poolDamage
	potionPool.PotionType = potionType
	$Projectiles.call_deferred("add_child", potionPool)
	
func SpawnEnemy():
	var enemy = ResourceLoader.load("res://Scenes/Enemies/Enemy.tscn")
	enemy.Position = GetNextPosition()

func GetNextPosition() -> Vector2:
	return Vector2.ONE
	

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

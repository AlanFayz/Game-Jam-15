extends Node

class GameComponents:
	var RandomNumbers: RandomNumberGenerator
	
var m_GameComponents = null
var m_SpawnZone = null

func _ready():
	m_GameComponents = GameComponents.new()
	m_GameComponents.RandomNumbers = RandomNumberGenerator.new()
	
	var mapSize = $Map.GetMapSizeInLocalSpace()
	var mapPosition = $Map.GetMapPositionInLocalSpace()
	
	m_SpawnZone = Rect2(mapPosition, mapSize)

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
	enemy.position = GetNextPosition()
	enemy.connect("FireBolt", OnEnemyFireBolt)
	$Enemies.call_deferred("add_child", enemy)

func OnPlayerSlash(slashPosition, slashDirection, slashDamage, slashType):
	var slash = ResourceLoader.load(slashType)
	slash.Position = slashPosition
	slash.AttackDir = slashDirection
	slash.Damage = slashDamage
	add_child(slash)

func OnEnemyFireBolt(position, direction, speed, damage):
	var fireBolt = ResourceLoader.load("res://Scenes/Projectiles/FireBolt.tscn")
	fireBolt.GlobalPosition = position
	fireBolt.Direction = direction
	fireBolt.Speed = speed
	fireBolt.Damage = damage
	$Projectiles.call_deferred("add_child", fireBolt)

func OnPlayerPlayerDeath():
	print("Player has died")

func GetNextPosition() -> Vector2:
	var start = m_SpawnZone.position;
	var end = m_SpawnZone.end;

	var position = Vector2(m_GameComponents.RandomNumbers.randf_range(start.x, end.x),
						   m_GameComponents.RandomNumbers.randf_range(start.y, end.y));
	return position;


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if $Enemies/Timer.time_left <= 0.0:
		for i in range(0, 10):
			SpawnEnemy();

		$Enemies/Timer.start()

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
	var potion = preload("res://Scenes/Projectiles/Potion.tscn").instantiate()
	potion.position = position
	potion.Speed = speed
	potion.Direction = direction
	potion.PoolDamage = poolDamage
	potion.BreakDamage = breakDamage
	potion.PotionType = potionType
	$Projectiles.add_child(potion)
	potion.connect("PotionBreak", OnPotionPotionbreak)
	pass

func OnPotionPotionbreak(position, poolDamage, potionType):
	var potionPool = preload("res://Scenes/Projectiles/PotionPool.tscn").instantiate()
	potionPool.position = position
	potionPool.Damage = poolDamage
	potionPool.PotionType = potionType
	$Projectiles.call_deferred("add_child", potionPool)
	
func SpawnEnemy():
	var enemy = preload("res://Scenes/Enemies/Enemy.tscn").instantiate()
	enemy.position = GetNextPosition()
	enemy.connect("FireBolt", OnEnemyFireBolt)
	$Enemies.call_deferred("add_child", enemy)

func OnPlayerSlash(slashPosition, slashDirection, slashDamage, slashType):
	var slash = load(slashType).instantiate()
	slash.position = slashPosition
	slash.AttackDir = slashDirection
	slash.Damage = slashDamage
	add_child(slash)

func OnEnemyFireBolt(position, direction, speed, damage):
	var fireBolt = preload("res://Scenes/Projectiles/FireBolt.tscn").instantiate()
	fireBolt.global_position = position
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
func _process(_delta):
	if $Enemies/Timer.time_left <= 0.0:
		for i in range(0, 10):
			SpawnEnemy();

		$Enemies/Timer.start()


func OnPlayerPurificationPotionThrow(pos, dir, speed, radius):
	var PurificationPotion = preload("res://Scenes/Projectiles/PurificationPotion.tscn").instantiate()
	PurificationPotion.Radius = radius
	PurificationPotion.Speed = speed
	PurificationPotion.position = pos
	PurificationPotion.Direction = dir
	PurificationPotion.connect("CreatePurificationPool", OnPurificationPotionCreatePurificationPool)
	$Projectiles.call_deferred("add_child", PurificationPotion)
	
func OnPurificationPotionCreatePurificationPool(pos, radius):
	var PurificationPool = preload("res://Scenes/Projectiles/PurificationPool.tscn").instantiate()
	PurificationPool.position = pos
	PurificationPool.Radius = radius
	PurificationPool.connect("PurifyArea", OnPurificationPoolPurifyArea)
	$Projectiles.call_deferred("add_child", PurificationPool)

func OnPurificationPoolPurifyArea(pos, radius):
	print("Is Purifying")
	var map = $Map
	var TilePos = map.GetPositionInTileSpace(pos)
	print(pos)
	map.ChangeTilesToLight(TilePos, radius/16)
	

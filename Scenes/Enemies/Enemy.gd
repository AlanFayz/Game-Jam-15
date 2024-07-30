extends CharacterBody2D 

signal FireBolt(position: Vector2, direction: Vector2, speed: float, damage: float)

enum State
{
	Idle = 0, Move, Attack, Death
}

class EnemyState:
	var StateType: State
	var Direction: Vector2
	var Velocity: Vector2
	var Health: float

func SetHealth(value):
	m_EnemyState.Health = value
	print("Enemy Health = ", m_EnemyState.Health)
	CheckHealth()

class Nodes:
	var Parent: Node
	var PlayerNode: Player

class EnemyProperties:
	# in pixel space
	const Speed: float = 100.0
	const ViewRadius: float = 300.0
	const AttackRadius: float = 150.0
	const Damage: float = 10.0
	const ProjectileSpeed: float = 350.0


var m_IsAnimationPlaying = false
var m_Bounds: Rect2

var m_States: Dictionary
var m_RandomNumberGenerator : RandomNumberGenerator

var m_EnemyState: EnemyState 
var m_Nodes: Nodes

#Effects stuff
@onready var FireCountdown = $Timers/FireTimeLeft
@onready var FireTicks = $Timers/FireTicks
@onready var PoisonCountdown = $Timers/PoisonTimeLeft
@onready var PoisonTicks = $Timers/PoisonTicks
@onready var FreezeCountdown = $Timers/FreezeTimeLeft
@onready var FreezeImmunityTimer = $Timers/FreezeImmunity

var IsOnFire: bool = false
var IsPoisoned: bool = false
var FireDamage: float = 0
var PoisonDamage: float = 0
var PoisonWeaknessLevel: float = 0
var SlowLevel: float = 0
var IsSlowed: bool = false

var FreezeImmunity: bool = false
func SetFreezeImmunity(value):
	if value:
		FreezeImmunityTimer.Start()

func Kill():
	m_EnemyState.StateType = State.Death
	m_IsAnimationPlaying = false
	$CollisionShape2D.set_deferred("disabled", true)
	$AnimationPlayer.play("Death")

func _ready():
	m_Nodes = Nodes.new()
	m_States = {}
	m_RandomNumberGenerator = RandomNumberGenerator.new()
	m_EnemyState = EnemyState.new()

	m_Nodes.Parent = get_parent().get_parent()	
	m_Nodes.PlayerNode = m_Nodes.Parent.get_node("Player") 

	var map = m_Nodes.Parent.get_node("Map")

	m_Bounds = Rect2(map.GetMapPositionInLocalSpace(), map.GetMapSizeInLocalSpace())

	m_EnemyState.StateType = State.Idle
	m_EnemyState.Velocity = Vector2.ZERO
	m_EnemyState.Health = 100.0

	InitalizeStates()

	m_States[State.Idle].call(EnemyProperties.Speed)


func _process(_delta):
	if m_EnemyState.StateType != State.Death:
		if $Timer.time_left == 0:
			m_EnemyState.StateType = ChooseState()
			m_IsAnimationPlaying = false
	
		if self.global_position.distance_to(GetGlobalPlayerPosition()) <= EnemyProperties.ViewRadius:
			m_EnemyState.StateType = State.Attack;
			m_IsAnimationPlaying = false;
		

		m_States[m_EnemyState.StateType].call(EnemyProperties.Speed);

		self.velocity = m_EnemyState.Velocity*GetFreezeSlowdown();
		

		move_and_slide();

		self.position = ClampPosition(self.position)
		$LaughAudioPlayer.position = self.position
		
		if $LaughAudioTimer.time_left == 0.0:
			$LaughAudioTimer.start()
			$LaughAudioPlayer.play()
		
	elif $Timer.time_left == 0:
		queue_free()
		return

func Hit(_origin: Node, damage: float, effects):
	SetHealth(m_EnemyState.Health - damage)
	
	#Freeze
	if effects[0] > 2 and not IsSlowed:
		IsSlowed = true
		SlowLevel = effects[0]
		FreezeCountdown.start(0.75)
	elif (effects[0] > 0 && !IsSlowed):
		IsSlowed = true
		FreezeCountdown.start(2.5)
		SlowLevel = effects[0]
		
	#Burn
	if (effects[1] > 0):
		IsOnFire = true
		FireCountdown.start(1.5+(0.5*effects[1]))
		FireDamage = 2+effects[1]
		FireTicks.start()
	
	#Poison
	if (effects[2] > 0):
		IsPoisoned = true;
		PoisonCountdown.start(5+2*effects[2])
		PoisonDamage = 3+0.75*effects[1]
		PoisonTicks.start()
		PoisonWeaknessLevel = effects[2]

func CheckHealth():
	if m_EnemyState.Health <= 0:
		Kill();


func ProcessIdleStateFun(_delta):
	if !m_IsAnimationPlaying:
		m_EnemyState.Direction = ChooseDirection()
		m_EnemyState.Velocity = Vector2.ZERO

		FlipSpriteIfNeeded()

		$AnimationPlayer.play("Idle")
		$Timer.start(5)

		m_IsAnimationPlaying = true

func ProcessMoveStateFun(delta):
	if !m_IsAnimationPlaying:
		m_EnemyState.Direction = ChooseDirection();

		FlipSpriteIfNeeded();

		$AnimationPlayer.play("Moving");
		$Timer.start(5);

		m_IsAnimationPlaying = true;
		

	m_EnemyState.Velocity = m_EnemyState.Direction * delta;
	
func ProcessAttackStateFun(delta):
	var direction = GetGlobalPlayerPosition() - self.global_position
	
	m_EnemyState.Direction =  direction.normalized()

	if self.position.distance_to(GetGlobalPlayerPosition()) >= EnemyProperties.AttackRadius:
		m_EnemyState.Velocity = m_EnemyState.Direction * delta
	else:
		m_EnemyState.Velocity = Vector2(0.0, 0.0)

	FlipSpriteIfNeeded()

	if !m_IsAnimationPlaying:
		$AnimationPlayer.play("Attacking")
		$Timer.start(2)
		
		m_IsAnimationPlaying = true;

func ProcessDeathStateFun(_delta):
	if !m_IsAnimationPlaying:
		$AnimationPlayer.Play("Death")
		$Timer.Start(5)
		m_IsAnimationPlaying = true

func InitalizeStates():
	var ProcessIdleState = ProcessIdleStateFun
	var ProcessMoveState = ProcessMoveStateFun	
	var ProcessAttackState = ProcessAttackStateFun
	var ProcessDeathState = ProcessDeathStateFun

	m_States[State.Idle]   = ProcessIdleState
	m_States[State.Attack] = ProcessAttackState
	m_States[State.Move]   = ProcessMoveState
	m_States[State.Death]  = ProcessDeathState


func ChooseDirection() -> Vector2:
	return Vector2(m_RandomNumberGenerator.randf_range(-1.0, 1.0),
				   m_RandomNumberGenerator.randf_range(-1.0, 1.0))


func FlipSpriteIfNeeded():
	if m_EnemyState.Direction.x < 0:
		$Sprite2D.flip_h = true;
	else:
		$Sprite2D.flip_h = false;


func ChooseState() -> State:
	return m_RandomNumberGenerator.randi_range(0, 1) as State

func GetGlobalPlayerPosition() -> Vector2:
	return m_Nodes.PlayerNode.global_position

func GetLocalPlayerPosition() -> Vector2:
	return m_Nodes.PlayerNode.position 

func Shoot():
	var direction = (GetGlobalPlayerPosition() - $ShootPoint.global_position)

	emit_signal(
		FireBolt.get_name(),
		$ShootPoint.global_position,
		direction.normalized(), 
		EnemyProperties.ProjectileSpeed, 
		EnemyProperties.Damage * GetPoisonWeakness())


func ClampPosition(pos: Vector2) -> Vector2:
	var clampedX = clamp(pos.x, m_Bounds.position.x, m_Bounds.end.x)
	var clampedY = clamp(pos.y, m_Bounds.position.y, m_Bounds.end.y)

	return Vector2(clampedX, clampedY);
	
	

func GetFreezeSlowdown() -> float:
	return clampf(1-SlowLevel/3,0,1)

func GetPoisonWeakness() -> float:
	return 1/clampf(PoisonWeaknessLevel*2/3, 1, 2^32);

func OnFireTimeLeftTimeout():
	IsOnFire = false
	FireTicks.stop()


func OnFreezeTimeLeftTimeout():
	IsSlowed = false
	SlowLevel = 0
	FreezeImmunity = true


func OnPoisonTimeLeftTimeout():
	IsPoisoned = false
	PoisonWeaknessLevel = 0
	PoisonTicks.stop()
	print("Stopped Poison")


func OnFireTicksTimeout():
	SetHealth(m_EnemyState.Health - FireDamage) 


func OnPoisonTicksTimeout():
	SetHealth(m_EnemyState.Health - PoisonDamage) 

func OnFreezeImmunityTimeout():
	FreezeImmunity = false



